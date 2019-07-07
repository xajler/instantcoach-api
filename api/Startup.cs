﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerGen;
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
            services.AddHeaderApiVersioning();
            services.AddVersionedApiExplorer();
            services.AddConfigOptionsService(Configuration);
            Config config = Configuration.GetSection(Config.Name).Get<Config>();
            services.AddDbcontextService(config.GetConnectionString());
            services.AddJwtAuthenticationService(config);
            services.AddSwaggerService();
            services.AddScoped(typeof(Repository<>));
            services.AddScoped<InstantCoachRepository>();
            services.AddScoped<IInstantCoachService, InstantCoachService>();
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        }

        public void Configure(IApplicationBuilder app, IApiVersionDescriptionProvider provider)
        {
            RunDbMigrationsAndSeedDataIfNeeded(app);
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<ResponseTimeMiddleware>();
            app.UseSwagger();
            // More info: https://github.com/microsoft/aspnet-api-versioning/tree/master/samples/aspnetcore/SwaggerSample
            app.UseSwaggerUIWAsHomeRoute(provider);
            app.UseAuthentication();
            // app.UseHttpsRedirection();
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
