using ControlFinWeb.App.AutoMapper;
using ControlFinWeb.App.Configurations;
using ControlFinWeb.App.Utilitarios;
using ControlFinWeb.Repositorio.Interface;
using ControlFinWeb.Repositorio.Servicos;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace ControlFinWeb.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddHttpContextAccessor();
            services.AddCookieConfiguration();
            services.AddMvcConfiguration();

            services.AddNHibernate();
            services.AddControllersWithViews();
            services.AddControllers();

            services.AddSession(options =>
            {
                options.IdleTimeout = System.TimeSpan.FromHours(2);
            });

            services.AddAutoMapper(typeof(AutoMapperSetup));
            services.AddScoped<IDbBackupService, DbBackupService>();
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), @"Configurations\DirectoryKeys")));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHttpContextAccessor accessor)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
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

            app.UseGlobalizationConfig();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Login}/{action=Entrar}/{id?}");

            });


        }
    }
}
