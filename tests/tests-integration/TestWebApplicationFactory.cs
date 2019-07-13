using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Api;
using Core;
using Core.Context;

namespace Tests.Integration
{
    public class TestWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("SUT");
            builder.ConfigureServices(services =>
            {
                var serviceProvider = new ServiceCollection()
                        .AddEntityFrameworkInMemoryDatabase()
                        .BuildServiceProvider();

                services.AddDbcontextService(Config.GetSUTConnectionString());

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<ICContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<TestWebApplicationFactory<TStartup>>>();

                    //db.Database.EnsureCreated();

                    // try
                    // {
                    //     db.EnsureSeeded();
                    // }
                    // catch (Exception ex)
                    // {
                    //     logger.LogError(ex, "An error occurred seeding the " +
                    //         $"database with test messages. Error: {ex.Message}");
                    // }
                }
            });
        }
    }
}