using ControlFinWeb.Dominio.Entidades;
using NHibernate;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioGasto : RepositorioBase<Gasto>
    {
        public RepositorioGasto(ISession session) : base(session) { }
    }
}
