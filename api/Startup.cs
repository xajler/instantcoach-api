using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Core.Context;
using Core;
using Core.Contracts;
using Core.Repositories;
using Core.Services;
using static System.Console;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            WriteLine($"Env is: {env.EnvironmentName}");
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWebApiService();
            services.AddApiVersioning();
            services.AddConfigOptionsService(Configuration);
            Config config = Configuration.GetSection(Config.Name).Get<Config>();
            services.AddDbcontextService(config.GetConnectionString());
            services.AddSwaggerService();
            services.AddSingleton<IInstantCoachRepository, InstantCoachRepository>();
            services.AddSingleton<IInstantCoachService, InstantCoachServices>();
        }

        public void Configure(IApplicationBuilder app)
        {
            RunDbMigrationsAndSeedDataIfNeeded(app);
            //app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(s => { s.SwaggerEndpoint("/swagger/v1/swagger.json", "InstantCoach API v1.0"); });
            app.UseMvc();
        }

        private void RunDbMigrationsAndSeedDataIfNeeded(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                             .GetRequiredService<IServiceScopeFactory>()
                             .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ICContext>();
                if (!context.AllMigrationsApplied())
                {
                    context.Database.Migrate();
                    context.EnsureSeeded();
                }
            }
        }
    }
}
