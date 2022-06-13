using ControlFinWeb.Dominio.Entidades;
using NHibernate;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioContaBancaria : RepositorioBase<ContaBancaria>
    {
        public RepositorioContaBancaria(ISession session) : base(session) { }
    }
}
