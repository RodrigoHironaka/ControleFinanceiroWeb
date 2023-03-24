using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Repositorio.Repositorios;
using NHibernate;
using ObjectsComparer;
using System.Collections.Generic;
using System;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioConta : RepositorioBase<Conta>
    {
        private readonly RepositorioLogModificacao RepositorioLog;
        public RepositorioConta(ISession session, RepositorioLogModificacao repositorioLog) : base(session) { RepositorioLog = repositorioLog; }

        public void EditarRegistrarLog(Conta conta, IEnumerable<Difference> diferencas, Usuario usuario, String chave)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    RepositorioLog.RegistrarLogModificacao(diferencas, usuario, chave);
                    AlterarLote(conta);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        public void ExcluirRegistrarLog(Conta conta, Usuario usuario)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    RepositorioLog.RegistrarLogExclusao($"Registro {conta.Nome}[{conta.Id}] excluído!", usuario, $"{conta.GetType().Name}[{conta.Id}]");
                    ExcluirLote(conta);
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
