using System;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Core;
using Core.Domain;

namespace Api
{
    public class ExceptionMiddleware
    {
        private const string PossibleBugText = "****POSSIBLE BUG******.";
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
            catch (DomainAssertionException ex)
            {
                _logger.LogError(ex,
                    "Domain Failure, data sent is not correct.Exception of Type: {ExceptionType} and Message: {Message}", ex.GetType().Name, ex.Message);

                await HandleDomainExceptionAsync(httpContext, ex.Message);
            }
            catch (DbUpdateException ex)
            {
                OnDbUpdateException(ex);
                await HandleExceptionAsync(httpContext);
            }
            catch (SqlException ex)
            {
                OnDbException(ex);
                await HandleExceptionAsync(httpContext);
            }
            catch (Exception ex) when (IsUsualExceptionsFilter(ex))
            {
                _logger.LogError(ex,
                    "{BugText} Exception of Type: {ExceptionType} and Message: {Message}",
                    PossibleBugText, ex.GetType().Name, ex.Message);
                await HandleExceptionAsync(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex,
                    "Unknown exception on server of Type: {ExceptionType} and Message: {Message}",
                    ex.GetType().Name, ex.Message);
                await HandleExceptionAsync(httpContext);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync("Internal Server Error. Something went wrong on server.");
        }

        private static Task HandleDomainExceptionAsync(HttpContext context, string message)
        {
            context.Response.ContentType = "text/plain";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return context.Response.WriteAsync($"Domain error message: {message}");
        }

        private void OnDbUpdateException(DbUpdateException ex)
        {
            if (ex.InnerException != null && ex.InnerException is SqlException)
            {
                OnDbException(ex.InnerException as SqlException);
            }
            else
            {
                _logger.LogCritical(ex, "Failed to Save DB Changes");
            }
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
                    "{BugText} Database error: {DbError}.", PossibleBugText, dbError);
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