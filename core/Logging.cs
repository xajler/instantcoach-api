using System;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Serilog.Exceptions;

namespace Core
{
    public static class Logging
    {
        public static ILogger Logger()
        {
            string elasticUri = "http://localhost:9200";

            Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(
                new Uri(elasticUri))
                {
                    AutoRegisterTemplate = true,
                })
            .CreateLogger();

            return Log.Logger;
        }
    }
}