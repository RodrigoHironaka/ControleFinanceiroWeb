using ControlFinWeb.Dominio.Entidades;
using NHibernate;
using ObjectsComparer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioCaixa : RepositorioBase<Caixa>
    {
       private readonly RepositorioFluxoCaixa RepositorioFluxoCaixa;
       private readonly RepositorioLogModificacao RepositorioLog;
        public RepositorioCaixa(ISession session, RepositorioFluxoCaixa repositorioFluxoCaixa, RepositorioLogModificacao repositorioLog) : base(session)
        {
            RepositorioFluxoCaixa = repositorioFluxoCaixa;
            RepositorioLog = repositorioLog;
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

        public void EditarRegistrarLog(Caixa caixa, IEnumerable<Difference> diferencas, Usuario usuario, String chave)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    RepositorioLog.RegistrarLogModificacao(diferencas, usuario, chave);
                    AlterarLote(caixa);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        public void ExcluirRegistrarLog(Caixa caixa, Usuario usuario)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    RepositorioLog.RegistrarLog($"Caixa [{caixa.Numero}] excluído!", usuario, $"Caixa[{caixa.Id}]");
                    ExcluirLote(caixa);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.Message);
                }
            }


        }
    }
}
