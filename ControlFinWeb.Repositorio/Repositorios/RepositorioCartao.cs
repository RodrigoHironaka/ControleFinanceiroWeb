using ControlFinWeb.Dominio.Entidades;
using NHibernate;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioCartao : RepositorioBase<Cartao>
    {
        public RepositorioCartao(ISession session) : base(session) { }
    }
    
}
