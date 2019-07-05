using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Api
{
    public class ResponseTimeMiddleware
    {
        private const string ResonseTimeHeaderName = "X-Response-Time";
        private readonly RequestDelegate _next;
        private readonly ILogger<ResponseTimeMiddleware> _logger;
        private readonly IHostingEnvironment _env;

        public ResponseTimeMiddleware(RequestDelegate next,
            ILogger<ResponseTimeMiddleware> logger,
            IHostingEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Use it in all environments EXCEPT 'Production'
            if (context.Request.Path.Value.Contains("api")
                && !_env.IsProduction())
            {
                var path = context.Request.Path.Value;
                var method = context.Request.Method;
                _logger.LogInformation($"\n\n----- START {method} {path} ---------------\n\n");
                var watch = Stopwatch.StartNew();

                context.Response.OnStarting(() =>
                {
                    watch.Stop();
                    var responseTime = watch.ElapsedMilliseconds;
                    context.Response.Headers[ResonseTimeHeaderName] = $"{responseTime}ms";
                    _logger.LogInformation($"Response time: {responseTime}ms");
                    _logger.LogInformation($"\n\n----- END {method} {path} ---------------\n\n");
                    return Task.CompletedTask;
                });
            }
            await _next(context);
        }
    }
}