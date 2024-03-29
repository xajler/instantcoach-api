﻿using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Core;
using static System.Environment;

namespace Api
{
    public static class Program
    {
        private static readonly string AspNetEnv =
            GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{AspNetEnv}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        public static async Task<int> Main()
        {
            var esUrl = GetEnvironmentVariable(Constants.EsUrlEnVar);
            Log.Logger = new Logging(Configuration, esUrl).Logger;
            try
            {
                Log.Information("Starting web host");
                await CreateWebHostBuilder(new string[] {}).Build().RunAsync();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
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
