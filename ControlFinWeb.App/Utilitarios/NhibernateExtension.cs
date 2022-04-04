using App1.Repositorio.Configuracao;
using ControlFinWeb.Repositorio.Repositorios;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace ControlFinWeb.App.Utilitarios
{
    public static class NhibernateExtension
    {
        public static IServiceCollection AddNHibernate(this IServiceCollection services)
        {
            var repositorios =
                from type in typeof(RepositorioUsuario).Assembly.GetExportedTypes()
                where type.FullName.StartsWith("ControlFinWeb.Repositorio.Repositorios")
                select new { type };

            foreach (var repositorio in repositorios)
                services.AddScoped(repositorio.type, repositorio.type);

            services.AddScoped(factory => NHibernateHelper.ObterSessao());

            return services;
        }
    }
}
