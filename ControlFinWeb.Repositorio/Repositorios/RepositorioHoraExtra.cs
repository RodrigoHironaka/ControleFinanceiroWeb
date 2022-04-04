using ControlFinWeb.Dominio.Entidades;
using NHibernate;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioHoraExtra : RepositorioBase<HoraExtra>
    {
        public RepositorioHoraExtra(ISession session) : base(session) { }
    }
}
