using ControlFinWeb.Dominio.Entidades;
using NHibernate;
using ObjectsComparer;
using System.Collections.Generic;
using System;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioUsuario : RepositorioBase<Usuario>
    {
        private readonly RepositorioLogModificacao RepositorioLog;
        public RepositorioUsuario(ISession session, RepositorioLogModificacao repositorioLog) : base(session) { RepositorioLog = repositorioLog; }

        public void EditarRegistrarLog(Usuario usuarioAlterado, IEnumerable<Difference> diferencas, Usuario usuario, String chave)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    RepositorioLog.RegistrarLogModificacao(diferencas, usuario, chave);
                    AlterarLote(usuarioAlterado);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        public void ExcluirRegistrarLog(Usuario usuarioAlterado, Usuario usuario)
        {
            using (var trans = Session.BeginTransaction())
            {
                try
                {
                    RepositorioLog.RegistrarLog($"Registro {usuarioAlterado.Nome}[{usuarioAlterado.Id}] excluído!", usuario, $"Usuário[{usuarioAlterado.Id}]");
                    ExcluirLote(usuarioAlterado);
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
