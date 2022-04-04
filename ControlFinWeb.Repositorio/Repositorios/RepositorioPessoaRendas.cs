using ControlFinWeb.Dominio.Entidades;
using NHibernate;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioPessoaRendas : RepositorioBase<PessoaRendas>
    {
        public RepositorioPessoaRendas(ISession session) : base(session) { }
    }
}
