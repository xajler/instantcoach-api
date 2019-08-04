using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Core;

namespace Api
{
    public sealed class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ExceptionLogger _exceptionLogger;
        private readonly ILogger _logger;

        public ExceptionMiddleware(RequestDelegate next,
            ILoggerFactory loggerFactory, ExceptionLogger exceptionLogger)
        {
            _logger = loggerFactory.CreateLogger<ExceptionMiddleware>();
            _exceptionLogger = exceptionLogger;
            _next = next;
            _exceptionLogger = exceptionLogger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try { await _next(httpContext); }
            catch (Exception ex)
            {
                var logResult = _exceptionLogger.GetLogResult(ex);
                if (logResult.Level == LogLevel.Critical)
                {
                    _logger.LogCritical(ex, logResult.Message);
                }
                else { OnLogError(ex, logResult); }
                await HandleExceptionAsync(httpContext);
            }
        }

        private void OnLogError(Exception exception, LogResult logResult)
        {
            if (logResult.IsDbError)
            {
                _logger.LogError(exception, logResult.Message, logResult.DbError);
            }
            else
            {
                _logger.LogError(exception, logResult.Message,
                    exception.GetType().Name, exception.Message);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync("Internal Server Error. Something went wrong on server.");
        }
    }
}