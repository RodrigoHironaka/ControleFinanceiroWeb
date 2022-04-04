using ControlFinWeb.Dominio.Dominios;
using NHibernate;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioFormaPagamento : RepositorioBase<FormaPagamento>
    {
        public RepositorioFormaPagamento(ISession session) : base(session) { }
    }
}
