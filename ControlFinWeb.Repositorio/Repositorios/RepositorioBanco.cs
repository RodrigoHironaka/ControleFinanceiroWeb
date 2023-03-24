using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Repositorio.Repositorios;
using NHibernate;
using ObjectsComparer;
using System.Collections.Generic;
using System;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioBanco : RepositorioBase<Banco>
    {
        private readonly RepositorioLogModificacao RepositorioLog;
        public RepositorioBanco(ISession session, RepositorioLogModificacao repositorioLog = null) : base(session) { RepositorioLog = repositorioLog; }

        public void EditarRegistrarLog(Banco banco, IEnumerable<Difference> diferencas, Usuario usuario, String chave)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    RepositorioLog.RegistrarLogModificacao(diferencas, usuario, chave);
                    AlterarLote(banco);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        public void ExcluirRegistrarLog(Banco banco, Usuario usuario)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    RepositorioLog.RegistrarLogExclusao($"Registro {banco.Nome}[{banco.Id}] excluído!", usuario, $"Banco[{banco.Id}]");
                    ExcluirLote(banco);
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
