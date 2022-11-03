using ControlFinWeb.Dominio.Entidades;
using NHibernate;
using System;
using System.Linq;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioCaixa : RepositorioBase<Caixa>
    {
       private readonly RepositorioFluxoCaixa RepositorioFluxoCaixa;
        public RepositorioCaixa(ISession session, RepositorioFluxoCaixa repositorioFluxoCaixa) : base(session)
        {
            RepositorioFluxoCaixa = repositorioFluxoCaixa;
        }

        public Caixa ObterCaixaAberto(Int64 idUsuario)
        {
            return ObterPorParametros(x => x.Situacao == Dominio.ObjetoValor.SituacaoCaixa.Aberto && x.UsuarioCriacao.Id == idUsuario).FirstOrDefault();
        }

        public void SalvarCaixa(Caixa caixa, Usuario usuario)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    SalvarLote(caixa);
                    RepositorioFluxoCaixa.GerarFluxoCaixa(caixa, usuario);
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
