using System;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Serilog.Exceptions;

namespace Api
{
    public sealed class Logging
    {
        private const string OutputFormat = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}";

        public Logging(IConfiguration config, string esUrl)
        {
           var logConfig = new LoggerConfiguration();
            logConfig.ReadFrom.Configuration(config)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console(outputTemplate: OutputFormat);

            if (!string.IsNullOrWhiteSpace(esUrl))
            {
                logConfig.WriteTo.Elasticsearch(
                    new ElasticsearchSinkOptions(
                        new Uri(esUrl)) { AutoRegisterTemplate = true });
            }

            Logger = logConfig.CreateLogger();
        }

        public ILogger Logger { get; }
    }
}