﻿using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate;
using System;
using System.Linq;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioFaturaItens : RepositorioBase<FaturaItens>
    {
        private readonly RepositorioParcela RepositorioParcela;
        private readonly RepositorioFatura RepositorioFaturaCartaoCredito;

        public RepositorioFaturaItens(RepositorioParcela repositorioParcela, RepositorioFatura repositorioFaturaCartaoCredito, ISession session) : base(session)
        {
            RepositorioParcela = repositorioParcela;
            RepositorioFaturaCartaoCredito = repositorioFaturaCartaoCredito;
        }

        public void SalvarAlterarFaturaItemEAlterarParcela(FaturaItens faturaCartaoCreditoItens, String nParcelas, Boolean replicar)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    var numParcelas = Int32.Parse(nParcelas);
                    Decimal valorParcela = 0, restante = 0;
                    String codItemRelacionado = String.Empty;

                    if (replicar)
                        valorParcela = faturaCartaoCreditoItens.Valor;
                    else
                    {
                        valorParcela = Math.Round(faturaCartaoCreditoItens.Valor / numParcelas, 2);
                        restante = Math.Round(faturaCartaoCreditoItens.Valor - (valorParcela * numParcelas), 2);
                    }

                    for (int i = 0; i < numParcelas; i++)
                    {
                        if (restante > 0 && i == numParcelas - 1)
                        {
                            valorParcela += restante;
                            restante = 0;
                        }

                        if (i == 0)
                        {
                            faturaCartaoCreditoItens.Valor = valorParcela;
                            faturaCartaoCreditoItens.QuantidadeRelacionado = faturaCartaoCreditoItens.Id == 0 ? String.Format("{0}/{1}", i + 1, numParcelas) : faturaCartaoCreditoItens.QuantidadeRelacionado;
                            SalvarOuAlterar(faturaCartaoCreditoItens);

                            if (String.IsNullOrEmpty(faturaCartaoCreditoItens.CodigoItemRelacionado))
                            {
                                codItemRelacionado = String.Format("{0}{1}", faturaCartaoCreditoItens.CartaoCredito.Id, faturaCartaoCreditoItens.Id);
                                faturaCartaoCreditoItens.CodigoItemRelacionado = codItemRelacionado;
                                AlterarLote(faturaCartaoCreditoItens);
                            }
                        }
                        else
                        {
                            var proximaFatura = RepositorioFaturaCartaoCredito.ObterPorParametros(x => x.Cartao == faturaCartaoCreditoItens.CartaoCredito.Cartao
                            && x.SituacaoFatura == SituacaoFatura.Aberta
                            && x.MesAnoReferencia == faturaCartaoCreditoItens.CartaoCredito.MesAnoReferencia.AddMonths(i)).FirstOrDefault();

                            var novoItemFatura = new FaturaItens()
                            {
                                Nome = faturaCartaoCreditoItens.Nome,
                                Valor = valorParcela,
                                DataCompra = faturaCartaoCreditoItens.DataCompra,
                                SubGasto = faturaCartaoCreditoItens.SubGasto,
                                Pessoa = faturaCartaoCreditoItens.Pessoa,
                                UsuarioCriacao = faturaCartaoCreditoItens.UsuarioCriacao,
                                CodigoItemRelacionado = codItemRelacionado,
                                QuantidadeRelacionado = String.Format("{0}/{1}", i + 1, numParcelas)
                            };

                            if (proximaFatura == null)
                            {
                                var novaFatura = new Fatura()
                                {
                                    MesAnoReferencia = faturaCartaoCreditoItens.CartaoCredito.MesAnoReferencia.AddMonths(i),
                                    Cartao = faturaCartaoCreditoItens.CartaoCredito.Cartao,
                                    UsuarioCriacao = faturaCartaoCreditoItens.UsuarioCriacao,
                                };
                                RepositorioFaturaCartaoCredito.SalvarEGerarNovaParcelaLote(novaFatura);
                                novoItemFatura.CartaoCredito = novaFatura;
                                SalvarOuAlterar(novoItemFatura);
                            }
                            else
                            {
                                novoItemFatura.CartaoCredito = proximaFatura;
                                SalvarOuAlterar(novoItemFatura);
                            }
                        }
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.ToString());
                }
            }
        }

        public void ExcluirItemFaturaEAlterarParcela(Int64 Id)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    var faturaCartaoCreditoItem = ObterPorId(Id);
                    faturaCartaoCreditoItem.CartaoCredito.FaturaItens.Remove(faturaCartaoCreditoItem);
                    RepositorioParcela.AlterarParcelaFatura(faturaCartaoCreditoItem, true);
                    ExcluirLote(faturaCartaoCreditoItem);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.ToString());
                }
            }
        }

        public void SalvarOuAlterar(FaturaItens faturaCartaoCreditoItem)
        {
            if (faturaCartaoCreditoItem.Id > 0)
            {
                RepositorioParcela.AlterarParcelaFatura(faturaCartaoCreditoItem, false, true);
                AlterarLote(faturaCartaoCreditoItem);
            }
            else
            {
                RepositorioParcela.AlterarParcelaFatura(faturaCartaoCreditoItem, false, false);
                SalvarLote(faturaCartaoCreditoItem);
            }

        }

    }
}
