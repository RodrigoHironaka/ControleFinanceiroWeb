using ControlFinWeb.App.AutoMapper;
using ControlFinWeb.App.Utilitarios;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Globalization;

namespace ControlFinWeb.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddHttpContextAccessor();

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
                
            services.AddNHibernate();
            services.AddControllersWithViews();
            services.AddControllers();

            services.AddSession(options =>
            {
                options.IdleTimeout = System.TimeSpan.FromHours(2);
            });

            services.AddAutoMapper(typeof(AutoMapperSetup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpContextAccessor accessor)
        {
            #region Definindo Cultura Pt-Br
            var defaultCulture = new CultureInfo("pt-BR");
            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(defaultCulture),
                SupportedCultures = new List<CultureInfo> { defaultCulture },
                SupportedUICultures = new List<CultureInfo> { defaultCulture }
            };
            app.UseRequestLocalization(localizationOptions);
            #endregion

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            Configuracao.SetHttpContextAcessor(accessor);

            app.UseCors(builder => builder.AllowAnyOrigin());

            app.UseNHibernateHttpModule();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSession();

            app.UseHttpsRedirection();

            app.UseCookiePolicy();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Login}/{action=Entrar}/{id?}");
            });


        }
    }
}
