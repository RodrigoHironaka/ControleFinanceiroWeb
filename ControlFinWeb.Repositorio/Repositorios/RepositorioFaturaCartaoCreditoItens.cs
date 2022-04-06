using ControlFinWeb.Dominio.Entidades;
using NHibernate;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioFaturaCartaoCreditoItens : RepositorioBase<FaturaCartaoCreditoItens>
    {
        public RepositorioFaturaCartaoCreditoItens(ISession session) : base(session) { }
    }
}
