using ControlFinWeb.Dominio.Entidades;
using NHibernate;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioCartaoCreditoItens : RepositorioBase<CartaoCreditoItens>
    {
        public RepositorioCartaoCreditoItens(ISession session) : base(session) { }
    }
}
