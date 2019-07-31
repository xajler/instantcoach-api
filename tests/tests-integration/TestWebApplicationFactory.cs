using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Serilog;
using Serilog.Exceptions;
using Serilog.Events;
using Api;
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
                        services.AddDbContextService(config.GetSutConnectionString());
                    });
        }

        private static Serilog.ILogger CreateLogger()
        {
            return new LoggerConfiguration()
                .MinimumLevel.Warning()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} " +
                                    "{Properties:j}{NewLine}{Exception}"
                ).CreateLogger();
        }
    }
}