using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate;
using ObjectsComparer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioFaturaItens : RepositorioBase<FaturaItens>
    {
        private readonly RepositorioParcela RepositorioParcela;
        private readonly RepositorioFatura RepositorioFatura;
        private readonly RepositorioLogModificacao RepositorioLog;

        public RepositorioFaturaItens(RepositorioParcela repositorioParcela, RepositorioFatura repositorioFatura, RepositorioLogModificacao repositorioLog, ISession session) : base(session)
        {
            RepositorioParcela = repositorioParcela;
            RepositorioFatura = repositorioFatura;
            RepositorioLog = repositorioLog;
        }

        public void SalvarAlterarFaturaItemEAlterarParcela(FaturaItens faturaItens, String nParcelas, Boolean replicar, IEnumerable<Difference> diferencas, Usuario usuario)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    var numParcelas = Int32.Parse(nParcelas);
                    Decimal valorParcela = 0, restante = 0;
                    String codItemRelacionado = String.Empty;

                    if (replicar)
                        valorParcela = faturaItens.Valor;
                    else
                    {
                        valorParcela = Math.Round(faturaItens.Valor / numParcelas, 2);
                        restante = Math.Round(faturaItens.Valor - (valorParcela * numParcelas), 2);
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
                            faturaItens.Valor = valorParcela;
                            faturaItens.QuantidadeRelacionado = faturaItens.Id == 0 ? String.Format("{0}/{1}", i + 1, numParcelas) : faturaItens.QuantidadeRelacionado;
                            SalvarOuAlterar(faturaItens, diferencas, usuario);

                            if (String.IsNullOrEmpty(faturaItens.CodigoItemRelacionado))
                            {
                                codItemRelacionado = String.Format("{0}{1}", faturaItens.Fatura.Id, faturaItens.Id);
                                faturaItens.CodigoItemRelacionado = codItemRelacionado;
                                AlterarLote(faturaItens);
                            }
                        }
                        else
                        {
                            var proximaFatura = RepositorioFatura.ObterPorParametros(x => x.Cartao == faturaItens.Fatura.Cartao
                            && x.SituacaoFatura == SituacaoFatura.Aberta
                            && x.MesAnoReferencia == faturaItens.Fatura.MesAnoReferencia.AddMonths(i)).FirstOrDefault();

                            var novoItemFatura = new FaturaItens()
                            {
                                Nome = faturaItens.Nome,
                                Valor = valorParcela,
                                DataCompra = faturaItens.DataCompra,
                                SubGasto = faturaItens.SubGasto,
                                Pessoa = faturaItens.Pessoa,
                                UsuarioCriacao = faturaItens.UsuarioCriacao,
                                CodigoItemRelacionado = codItemRelacionado,
                                QuantidadeRelacionado = String.Format("{0}/{1}", i + 1, numParcelas)
                            };

                            if (proximaFatura == null)
                            {
                                var novaFatura = new Fatura()
                                {
                                    MesAnoReferencia = faturaItens.Fatura.MesAnoReferencia.AddMonths(i),
                                    Cartao = faturaItens.Fatura.Cartao,
                                    UsuarioCriacao = faturaItens.UsuarioCriacao,
                                };
                                RepositorioFatura.SalvarEGerarNovaParcelaLote(novaFatura);
                                novoItemFatura.Fatura = novaFatura;
                                SalvarOuAlterar(novoItemFatura, diferencas, usuario);
                            }
                            else
                            {
                                novoItemFatura.Fatura = proximaFatura;
                                SalvarOuAlterar(novoItemFatura, diferencas, usuario);
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

        public void ExcluirItemFaturaEAlterarParcela(Int64 Id, Usuario usuario)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    var faturaItem = ObterPorId(Id);
                    faturaItem.Fatura.FaturaItens.Remove(faturaItem);
                    RepositorioParcela.AlterarParcelaFatura(faturaItem, true);
                    ExcluirLote(faturaItem);
                    RepositorioLog.RegistrarLog($"Item {faturaItem.Nome} da Fatura {faturaItem.Fatura._DescricaoCompleta} excluído", usuario, $"Fatura[{faturaItem.Fatura.Id}] - Item[{faturaItem.Id}]");
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.ToString());
                }
            }
        }

        public void SalvarOuAlterar(FaturaItens faturaItem, IEnumerable<Difference> diferencas, Usuario usuario)
        {
            if (faturaItem.Id > 0)
            {
                RepositorioParcela.AlterarParcelaFatura(faturaItem, false, true);
                AlterarLote(faturaItem);
                RepositorioLog.RegistrarLogModificacao(diferencas, usuario, $"Fatura[{faturaItem.Fatura.Id}] - Item[{faturaItem.Id}]");
            }
            else
            {
                RepositorioParcela.AlterarParcelaFatura(faturaItem, false, false);
                SalvarLote(faturaItem);
            }

        }

    }
}
