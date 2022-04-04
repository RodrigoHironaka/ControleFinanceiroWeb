using ControlFinWeb.Dominio.Entidades;
using NHibernate;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioPessoa : RepositorioBase<Pessoa>
    {
        public RepositorioPessoa(ISession session) : base(session) { }

    }
}
