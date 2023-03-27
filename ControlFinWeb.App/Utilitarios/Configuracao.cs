using ControlFinWeb.Dominio.Entidades;
using ControlFinWeb.Repositorio.Repositorios;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;

namespace ControlFinWeb.App.Utilitarios
{
    public class Configuracao
    {
        private static IHttpContextAccessor httpContextAcessor;
       public static void SetHttpContextAcessor(IHttpContextAccessor acessor)
        {
            httpContextAcessor = acessor;
        }

        public static Usuario Usuario
        {
            get
            {
                if (String.IsNullOrEmpty(httpContextAcessor.HttpContext.Session?.GetString("Usuario")))
                {
                    Usuario usu = new RepositorioUsuario(NHibernateHttpModule.CurrentSession, null).ObterPorId(Int64.Parse(httpContextAcessor.HttpContext.User.Identity.Name));

                    Usuario = new Usuario
                    {
                        Id = usu.Id,
                        Nome = usu.Nome,
                        Imagem = usu.Imagem
                    };

                }
                return JsonConvert.DeserializeObject<Usuario>(httpContextAcessor.HttpContext.Session.GetString("Usuario"));
            }

            set
            {
                httpContextAcessor.HttpContext.Session.SetString("Usuario", JsonConvert.SerializeObject(value));
            }
        }
    }
}
