using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Core;
using static System.Console;
using static System.Environment;


namespace Api
{
    public class Program
    {
        private static readonly string AspNetEnv =
            GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{AspNetEnv}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static int Main(string[] args)
        {
            WriteLine($"Current path: {Directory.GetCurrentDirectory()}");
            var esUrl = GetEnvironmentVariable(Constants.EsUrlEnVar);
            WriteLine("ES URI: {0}", esUrl);
            Log.Logger = Logging.Logger(Configuration, esUrl);
            try
            {
                Log.Information("Starting web host");
                CreateWebHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex,
                    "Host terminated unexpectedly.\nStack trace:\n{0}",
                    ex.ToInnerMessagesDump());
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
             WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseStartup<Startup>();
    }
}
