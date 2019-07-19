using System;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Core;
using static Core.Constants;

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
                _logger.LogError(
                    "{BugText} Exception of Type: {ExceptionType} and Message: {Message}\nStack Trace:\n{StackTrace}",
                    PossibleBugText, ex.GetType().Name, ex.Message, ex.ToInnerMessagesDump());
                await HandleExceptionAsync(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(
                    "Unknown exception on server of Type: {ExceptionType} and Message: {Message}\nStack Trace:\n{StackTrace}",
                    ex.GetType().Name, ex.Message, ex.ToInnerMessagesDump());
                await HandleExceptionAsync(httpContext);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return context.Response.WriteAsync("Internal Server Error. Something went wrong on server.");
        }

        private void OnDbUpdateException(DbUpdateException ex)
        {
            if (ex.InnerException != null && ex.InnerException is SqlException)
            {
                OnDbException(ex.InnerException as SqlException);
            }
            else
            {
                _logger.LogCritical("Failed to Save DB Changes.\nStack Trace:\n{StackTrace}",
                    ex.ToInnerMessagesDump());
            }
        }

        private void OnDbException(SqlException ex)
        {
            var dbError = SqlServerErrorManager.GetError(ex);
            if (dbError == DatabaseError.Unhandled)
            {
                _logger.LogCritical(
                    "Unhandled DB exception. Possible connection error.\nStack Trace:\n{StackTrace}",
                        ex.ToInnerMessagesDump());
            }
            else
            {
                _logger.LogError(
                    "{BugText} Database error: {DbError}.\nStack Trace:\n{StackTrace}",
                    PossibleBugText, dbError, ex.ToInnerMessagesDump());
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