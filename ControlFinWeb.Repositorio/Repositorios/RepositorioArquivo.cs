using ControlFinWeb.Dominio.Entidades;
using NHibernate;

namespace ControlFinWeb.Repositorio.Repositorios
{
    public class RepositorioArquivo : RepositorioBase<Arquivo>
    {
        public RepositorioArquivo(ISession session) : base(session) { }
    }
}
