using ControlFinWeb.Dominio.Entidades;
using NHibernate;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioUsuario : RepositorioBase<Usuario>
    {
        public RepositorioUsuario(ISession session) : base(session) { }
    }
}
