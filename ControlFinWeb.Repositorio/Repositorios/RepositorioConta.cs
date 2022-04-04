using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Repositorio.Repositorios;
using NHibernate;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioConta : RepositorioBase<Conta>
    {
        public RepositorioConta(ISession session) : base(session) { }
    }
}
