using ControlFinWeb.Dominio.Entidades;
using NHibernate;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioSubGasto : RepositorioBase<SubGasto>
    {
        public RepositorioSubGasto(ISession session) : base(session) { }
    }
}
