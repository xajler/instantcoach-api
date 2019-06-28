using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Core.Context;
using Core;
using static System.Console;

namespace InstantCoach
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //
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
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            WriteLine($"Env is: {env.EnvironmentName}");
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            using (var serviceScope = app.ApplicationServices
                                         .GetRequiredService<IServiceScopeFactory>()
                                         .CreateScope())
            {
                serviceScope.ServiceProvider.GetService<ICContext>().Database.Migrate();
                if (!serviceScope.ServiceProvider.GetService<ICContext>().AllMigrationsApplied())
                {
                    serviceScope.ServiceProvider.GetService<ICContext>().Database.Migrate();
                    serviceScope.ServiceProvider.GetService<ICContext>().EnsureSeeded();
                }
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
