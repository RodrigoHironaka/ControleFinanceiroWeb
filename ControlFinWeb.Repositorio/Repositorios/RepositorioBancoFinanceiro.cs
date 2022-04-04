using ControlFinWeb.Dominio.Entidades;
using NHibernate;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioBancoFinanceiro : RepositorioBase<BancoFinanceiro>
    {
        public RepositorioBancoFinanceiro(ISession session) : base(session) { }
    }
}
