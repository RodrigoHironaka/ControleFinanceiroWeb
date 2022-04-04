using ControlFinWeb.Dominio.Entidades;
using NHibernate;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioCaixa : RepositorioBase<Caixa>
    {
        public RepositorioCaixa(ISession session) : base(session) { }
    }
}
