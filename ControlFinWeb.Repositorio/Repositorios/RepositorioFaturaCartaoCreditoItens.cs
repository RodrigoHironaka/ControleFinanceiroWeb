using ControlFinWeb.Dominio.Entidades;
using NHibernate;
using System;
using System.Linq;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioFaturaCartaoCreditoItens : RepositorioBase<FaturaCartaoCreditoItens>
    {
        private readonly RepositorioParcela RepositorioParcela;

        public RepositorioFaturaCartaoCreditoItens(RepositorioParcela repositorioParcela, ISession session) : base(session)
        {
            RepositorioParcela = repositorioParcela;
        }

        public void SalvarAlterarFaturaItemEAlterarParcela(FaturaCartaoCreditoItens faturaCartaoCreditoItens)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    RepositorioParcela.AlterarParcelaFatura(faturaCartaoCreditoItens);
                    if (faturaCartaoCreditoItens.Id > 0)
                    {
                        AlterarLote(faturaCartaoCreditoItens);
                    }
                    else
                    {
                        SalvarLote(faturaCartaoCreditoItens);
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
    }
}
