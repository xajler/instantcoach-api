using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using static System.Console;
using static Core.Constants;

namespace Api
{
    public sealed class ResponseTimeMiddleware
    {
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

                if (_env.EnvironmentName == LocalEnv)
                {
                    _logger.LogInformation("\n\n----- START {HttpMethod} {HttpPath} ---------------\n\n",
                    method, path);
                }

                var watch = Stopwatch.StartNew();

                context.Response.OnStarting(() =>
                {
                    watch.Stop();
                    var responseTime = watch.ElapsedMilliseconds;
                    context.Response.Headers[ResponseTimeHeader] = $"{responseTime}ms";
                    _logger.LogInformation("Request: {HttpMethod} {HttpPath} | Response time: {ResponseTime}ms",
                        method, path, responseTime);

                    if (_env.EnvironmentName == LocalEnv)
                    {
                        _logger.LogInformation("\n\n----- END {HttpMethod} {HttpPath} ---------------\n\n",
                        method, path);
                    }

                    return Task.CompletedTask;
                });
            }
            await _next(context);
        }
    }
}