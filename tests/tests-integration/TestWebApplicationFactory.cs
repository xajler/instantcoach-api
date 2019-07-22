using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using Serilog.Events;
using Api;
using Core.Context;
using static Tests.Integration.TestHelpers;
using static Core.Constants;

namespace Tests.Integration
{
    public class TestWebApplicationFactory<TStartup>
    : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // !!!! Remove comment if needed for more info when running ClientController tests. !!!
            Log.Logger = CreateLogger();
            builder.UseEnvironment(SUTEnv)
                   .ConfigureServices(services =>
                    {
                        var config = CreateConfigForTest();
                        services.AddDbContextService(config.GetSUTConnectionString());

                        var sp = services.BuildServiceProvider();

                        using (var scope = sp.CreateScope())
                        {
                            var scopedServices = scope.ServiceProvider;
                            var db = scopedServices.GetRequiredService<ICContext>();
                            var logger = scopedServices
                                .GetRequiredService<ILogger<TestWebApplicationFactory<TStartup>>>();
                        }
                    });
        }

        private static Serilog.ILogger CreateLogger()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} " +
                                    "{Properties:j}{NewLine}{Exception}"
                ).CreateLogger();
        }
    }
}