using ControlFinWeb.Dominio.Entidades;
using NHibernate;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioCartaoCredito : RepositorioBase<CartaoCredito>
    {
        public RepositorioCartaoCredito(ISession session) : base(session) { }
    }
}
