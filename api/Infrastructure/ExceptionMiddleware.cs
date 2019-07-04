using System;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Core;

namespace Api
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ExceptionMiddleware>();
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (SqlException ex)
            {
                OnDbException(ex);
                await HandleExceptionAsync(httpContext);
            }
            catch (Exception ex) when (IsUsualExceptionsFilter(ex))
            {
                _logger.LogError(ex,
                    $"Possible bug. Exception of Type: {ex.GetType().Name} and Message: {ex.Message}");
                await HandleExceptionAsync(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex,
                    $"Unknown exception on server of Type: {ex.GetType().Name} and Message: {ex.Message}");
                await HandleExceptionAsync(httpContext);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync("Internal Server Error. Something went wrong on server.");
        }

        private void OnDbException(SqlException ex)
        {
            var dbError = SqlServerErrorManager.GetError(ex);
            if (dbError == DatabaseError.Unhandled)
            {
                _logger.LogCritical(ex,
                    "Unhandled DB exception. Possible connection error.");
            }
            else
            {
                _logger.LogError(ex,
                    $"Possible bug. Database error: {dbError}.");
            }
        }

        private bool IsUsualExceptionsFilter(Exception ex)
        {
            return ex is IndexOutOfRangeException
                   || ex is InvalidOperationException
                   || ex is IndexOutOfRangeException
                   || ex is NullReferenceException
                   || ex is ArgumentOutOfRangeException
                   || ex is ArgumentNullException
                   || ex is ArgumentException;
        }
    }
}