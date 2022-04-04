using ControlFinWeb.Dominio.Entidades;
using NHibernate;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioRenda : RepositorioBase<Renda>
    {
        public RepositorioRenda(ISession session) : base(session) { }
    }
}
