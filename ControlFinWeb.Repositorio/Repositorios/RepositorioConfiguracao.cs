using ControlFinWeb.Repositorio.Mapeamentos;
using NHibernate;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioConfiguracao : RepositorioBase<Configuracao>
    {
        public RepositorioConfiguracao(ISession session) : base(session) { }
    }
}
