using ControlFinWeb.Dominio.Entidades;
using NHibernate;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioFaturaCartaoCredito : RepositorioBase<FaturaCartaoCredito>
    {
        public RepositorioFaturaCartaoCredito(ISession session) : base(session) { }
    }
}
