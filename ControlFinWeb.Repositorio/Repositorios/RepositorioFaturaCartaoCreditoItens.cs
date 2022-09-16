using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Dominio.ObjetoValor;
using NHibernate;
using System;
using System.Linq;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioFaturaCartaoCreditoItens : RepositorioBase<FaturaCartaoCreditoItens>
    {
        private readonly RepositorioParcela RepositorioParcela;
        private readonly RepositorioFaturaCartaoCredito RepositorioFaturaCartaoCredito;

        public RepositorioFaturaCartaoCreditoItens(RepositorioParcela repositorioParcela, RepositorioFaturaCartaoCredito repositorioFaturaCartaoCredito, ISession session) : base(session)
        {
            RepositorioParcela = repositorioParcela;
            RepositorioFaturaCartaoCredito = repositorioFaturaCartaoCredito;
        }

        //public void SalvarAlterarFaturaItemEAlterarParcela(FaturaCartaoCreditoItens faturaCartaoCreditoItens)
        //{
        //    using (var trans = Session.BeginTransaction())
        //    {
        //        try
        //        {
        //            RepositorioParcela.AlterarParcelaFatura(faturaCartaoCreditoItens);
        //            if (faturaCartaoCreditoItens.Id > 0)
        //            {
        //                AlterarLote(faturaCartaoCreditoItens);
        //            }
        //            else
        //            {
        //                SalvarLote(faturaCartaoCreditoItens);
        //            }

        //            trans.Commit();

        //        }
        //        catch (Exception ex)
        //        {
        //            trans.Rollback();
        //            throw new Exception(ex.ToString());
        //        }
        //    }
        //}

        public void SalvarAlterarFaturaItemEAlterarParcela(FaturaCartaoCreditoItens faturaCartaoCreditoItens, String nParcelas, Boolean replicar)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    var numParcelas = Int32.Parse(nParcelas);
                    Decimal valorParcela = 0, restante = 0;

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
                            RepositorioParcela.AlterarParcelaFatura(faturaCartaoCreditoItens);
                            SalvarOuAlterar(faturaCartaoCreditoItens);
                        }
                        else
                        {
                            var proximaFatura = RepositorioFaturaCartaoCredito.ObterPorParametros(x => x.Cartao == faturaCartaoCreditoItens.CartaoCredito.Cartao
                            && x.SituacaoFatura == SituacaoFatura.Aberta
                            && x.MesAnoReferencia == faturaCartaoCreditoItens.CartaoCredito.MesAnoReferencia.AddMonths(i)).FirstOrDefault();

                            var novoItemFatura = new FaturaCartaoCreditoItens()
                            {
                                Nome = faturaCartaoCreditoItens.Nome,
                                Valor = valorParcela,
                                DataCompra = faturaCartaoCreditoItens.DataCompra,
                                SubGasto = faturaCartaoCreditoItens.SubGasto,
                                Pessoa = faturaCartaoCreditoItens.Pessoa,
                                UsuarioCriacao = faturaCartaoCreditoItens.UsuarioCriacao
                            };

                            if (proximaFatura == null)
                            {
                                var novaFatura = new FaturaCartaoCredito()
                                {
                                    MesAnoReferencia = faturaCartaoCreditoItens.CartaoCredito.MesAnoReferencia.AddMonths(i),
                                    Cartao = faturaCartaoCreditoItens.CartaoCredito.Cartao,
                                    UsuarioCriacao = faturaCartaoCreditoItens.UsuarioCriacao,
                                };
                                RepositorioFaturaCartaoCredito.SalvarEGerarNovaParcelaLote(novaFatura);
                                novoItemFatura.CartaoCredito = novaFatura;
                                RepositorioParcela.AlterarParcelaFatura(novoItemFatura);
                                SalvarOuAlterar(novoItemFatura);
                            }
                            else
                            {
                                novoItemFatura.CartaoCredito = proximaFatura;
                                RepositorioParcela.AlterarParcelaFatura(novoItemFatura);
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
                    RepositorioParcela.AlterarParcelaFatura(faturaCartaoCreditoItem);
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

        public void SalvarOuAlterar(FaturaCartaoCreditoItens faturaCartaoCreditoItens)
        {
            if (faturaCartaoCreditoItens.Id > 0)
            {
                AlterarLote(faturaCartaoCreditoItens);
            }
            else
            {
                SalvarLote(faturaCartaoCreditoItens);
            }
        }
    }
}
