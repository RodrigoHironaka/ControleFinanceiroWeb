using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ControlFinWeb.App.Configurations
{
    public static class CookieConfig
    {
        public static IServiceCollection AddCookieConfiguration(this IServiceCollection services) 
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            {
                options.Cookie.Name = "CookieLoginControleFinanceiro";
                options.LoginPath = "/Login/Entrar";
            });
            return services;
        }
    }
}
