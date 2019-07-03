using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Swashbuckle.AspNetCore.Swagger;
using Core.Context;
using Core;
using Core.Contracts;
using Core.Repositories;
using Core.Services;
using Api.Filters;
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
            services.AddMvc(options =>
                {
                    options.Filters.Add<OperationCancelledExceptionFilter>();
                })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    options.SerializerSettings.Formatting = Formatting.None;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddApiVersioning();
            services.AddOptions<Config>()
                    .Configure(options => Configuration.GetSection(Config.Name).Bind(options))
                    .ValidateDataAnnotations();
            Config config = Configuration.GetSection(Config.Name).Get<Config>();
            // WriteLine($"conn string: {config.GetConnectionString()}");
            services.AddDbContext<ICContext>(options =>
                options.UseSqlServer(config.GetConnectionString(), providerOptions =>
                        {
                        providerOptions.CommandTimeout(180);
                        providerOptions.EnableRetryOnFailure(
                            maxRetryCount: 10,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                        })
                       .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "InstantCoach API",
                    Description = "A sample ASP.NET Core web API for microservices",
                    Contact = new Contact
                    {
                        Name = "Kornelije Sajler",
                        Email = "ks@metaintellect.com",
                        Url = "https://git.430n.com/x430n/instantcoach"
                    },
                    License = new License
                    {
                        Name = "MIT",
                        Url = "https://git.430n.com/x430n/instantcoach/src/branch/master/LICENSE"
                    }
                });
            });
            services.AddSingleton<IInstantCoachRepository, InstantCoachRepository>();
            services.AddSingleton<IInstantCoachService, InstantCoachServices>();
        }

        public void Configure(IApplicationBuilder app)
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

            //app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(s =>
            {
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "InstantCoach API v1.0");
            });
            app.UseMvc();
        }
    }
}
