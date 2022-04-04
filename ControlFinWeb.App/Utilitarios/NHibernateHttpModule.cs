using App1.Repositorio.Configuracao;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControlFinWeb.App.Utilitarios
{
    public class NHibernateHttpModule
    {
        private readonly RequestDelegate _next;

        private const string KEY = "_TheSession_";
        private static HttpContext currentContext;

        public NHibernateHttpModule(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            httpContext.Items[KEY] = NHibernateHelper.ObterSessao();
            currentContext = httpContext;

            await _next(httpContext);

            NHibernate.ISession session = httpContext.Items[KEY] as NHibernate.ISession;
            if (session != null)
            {
                try
                {
                    session.Flush();
                    session.Close();
                }
                catch { }
            }
            httpContext.Items[KEY] = null;
            currentContext.Items[KEY] = null;
        }

        public static NHibernate.ISession CurrentSession
        {
            get
            {
                NHibernate.ISession session = currentContext.Items[KEY] as NHibernate.ISession;
                if (session == null)
                {
                    session = NHibernateHelper.ObterSessao();
                    currentContext.Items[KEY] = session;
                }
                return session;
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class NHibernateHttpModuleExtensions
    {
        public static IApplicationBuilder UseNHibernateHttpModule(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<NHibernateHttpModule>();
        }
    }
}

