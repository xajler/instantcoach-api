using System;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using static Core.Constants;

namespace Api
{
    public enum DatabaseError
    {
        UniqueConstraint,
        CannotInsertNull,
        MaxLength,
        NumericOverflow,
        Unhandled
    }

    public static class SqlServerErrorManager
    {
        public static DatabaseError GetError(SqlException dbException)
        {
            const int CannotInsertNull = 515;
            const int CannotInsertDuplicateKeyUniqueIndex = 2601;
            const int CannotInsertDuplicateKeyUniqueConstraint = 2627;
            const int ArithmeticOverflow = 8115;
            const int StringOrBinaryDataWouldBeTruncated = 8152;

            switch (dbException.Number)
            {
                case CannotInsertNull:
                    return DatabaseError.CannotInsertNull;
                case CannotInsertDuplicateKeyUniqueIndex:
                case CannotInsertDuplicateKeyUniqueConstraint:
                    return DatabaseError.UniqueConstraint;
                case ArithmeticOverflow:
                    return DatabaseError.NumericOverflow;
                case StringOrBinaryDataWouldBeTruncated:
                    return DatabaseError.MaxLength;
                default:
                    return DatabaseError.Unhandled;
            }
        }
    }

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
                _logger.LogError(ex,
                    "{BugText} Exception of Type: {ExceptionType} and Message: {Message}",
                    PossibleBugText, ex.GetType().Name);
                await HandleExceptionAsync(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex,
                    "Unknown exception on server of Type: {ExceptionType} an Message: {Message}",
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

        private void OnDbUpdateException(DbUpdateException ex)
        {
            if (ex.InnerException != null && ex.InnerException is SqlException)
            {
                OnDbException(ex.InnerException as SqlException);
            }
            else
            {
                _logger.LogCritical(ex, "Failed to Save DB Changes.");
            }
        }

        private void OnDbException(SqlException ex)
        {
            var dbError = SqlServerErrorManager.GetError(ex);
            if (dbError == DatabaseError.Unhandled)
            {
                _logger.LogCritical(ex, "Unhandled DB exception. Possible connection error.");
            }
            else
            {
                _logger.LogError(ex,"{BugText} Database error: {DbError}.",
                    PossibleBugText, dbError);
            }
        }

        private static bool IsUsualExceptionsFilter(Exception ex)
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