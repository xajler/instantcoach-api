using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;
using Elastic.Apm.All;
using Core.Context;
using Core;
using Core.Repositories;
using Core.Services;
using static System.Console;
using static Core.Constants;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        public IConfiguration Configuration { get; set; }
        public IHostingEnvironment Env { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWebApiService();
            services.AddHttpContextAccessor();
            services.AddHeaderApiVersioning();
            services.AddVersionedApiExplorer();
            services.AddConfigOptionsService(Configuration);
            Config config = Configuration.GetSection(Config.Name).Get<Config>();

            if (Env.EnvironmentName == SUTEnv)
            {
                services.AddDbContextService(config.GetSUTConnectionString());
                services.AddFakeSUTJwtAuthenticationService();
            }
            else
            {

                services.AddDbContextService(config.GetConnectionString());
                services.AddJwtAuthenticationService(config);
            }

            services.AddSwaggerService();
            services.AddScoped(typeof(Repository<>));
            services.AddScoped<InstantCoachRepository>();
            services.AddScoped<IInstantCoachService, InstantCoachService>();
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        }

        public void Configure(IApplicationBuilder app,
            IApiVersionDescriptionProvider provider)
        {
            RunDbMigrationsAndSeedDataIfNeeded(app);
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<ResponseTimeMiddleware>();
            app.UseSwagger();
            // More info: https://github.com/microsoft/aspnet-api-versioning/tree/master/samples/aspnetcore/SwaggerSample
            app.UseSwaggerUIAsHomeRoute(provider);
            app.UseElasticApm(Configuration);
            app.UseAuthentication();
            app.UseMvc();
        }

        private static void RunDbMigrationsAndSeedDataIfNeeded(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                             .GetRequiredService<IServiceScopeFactory>()
                             .CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ICContext>();
                if (!context.AllMigrationsApplied())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
