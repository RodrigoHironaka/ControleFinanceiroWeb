using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Repositorio.Repositorios;
using NHibernate;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioBanco : RepositorioBase<Banco>
    {
        public RepositorioBanco(ISession session) : base(session) { }
    }
}
