using ControlFinWeb.Dominio.Entidades;
using NHibernate;
using System;
using System.Linq;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioFaturaCartaoCredito : RepositorioBase<FaturaCartaoCredito>
    {
        private readonly RepositorioParcela RepositorioParcela;
        private readonly RepositorioCartao RepositorioCartao;
        public RepositorioFaturaCartaoCredito(ISession session, RepositorioParcela repositorioParcela, RepositorioCartao repositorioCartao) : base(session)
        {
            RepositorioParcela = repositorioParcela;
            RepositorioCartao = repositorioCartao;
        }

        public void SalvarEGerarNovaParcela(FaturaCartaoCredito entidade)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    entidade.DataGeracao = DateTime.Now;
                    this.Session.Save(entidade);

                    var diaVenc = RepositorioCartao.ObterPorId(entidade.Cartao.Id).DiaVencimento;
                    var dataVenc = new DateTime(entidade.MesAnoReferencia.Year, entidade.MesAnoReferencia.AddMonths(1).Month, diaVenc);
                    RepositorioParcela.AdicionarNovaParcela(0, dataVenc, entidade.UsuarioCriacao, null, entidade);

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
