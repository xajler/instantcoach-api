using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Core.Context;

namespace InstantCoach
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<ICContext>(options => options.UseSqlServer(new ICContextFactory().CreateDbContext()));
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<ICContext>().Database.Migrate();
                if (!serviceScope.ServiceProvider.GetService<ICContext>().AllMigrationsApplied())
                {
                     serviceScope.ServiceProvider.GetService<ICContext>().Database.Migrate();
                     serviceScope.ServiceProvider.GetService<ICContext>().EnsureSeeded();
                }
            }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
