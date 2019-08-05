using System;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Core.Constants;

namespace Core
{
    public enum DatabaseError
    {
        UniqueConstraint,
        CannotInsertNull,
        MaxLength,
        NumericOverflow,
        Unhandled
    }

    public class LogResult
    {
        public LogResult(LogLevel level, string message)
        {
            Level = level;
            Message = message;
        }
        public LogResult(LogLevel level, string message, DatabaseError dbError)
        {
            Level = level;
            Message = message;
            DbError = dbError;
        }
        public LogLevel Level { get; private set; }
        public string Message { get; private set; }
        public DatabaseError? DbError { get; private set; }
        public bool IsDbError => DbError != null;
    }

    public class ExceptionLogger
    {
        public LogResult GetLogResult(Exception exception)
        {
            switch (exception)
            {
                case DbUpdateException duEx:
                    return OnDbUpdateException(duEx);
                case SqlException sEx:
                    return OnDbException(sEx);
                case Exception ex when IsUsualExceptionsFilter(ex):
                    return new LogResult(LogLevel.Error,
                        $"{PossibleBugText} {GenericExceptionMsg}");
                default:
                    return new LogResult(LogLevel.Critical, UnhandledExceptionMsg);
            }
        }

        private static LogResult OnDbUpdateException(DbUpdateException ex)
        {

            if (ex.InnerException != null && ex.InnerException is SqlException)
            {
                return OnDbException(ex.InnerException as SqlException);
            }
            else
            {
                var msg = $"Failed to Save DB Changes. {GenericExceptionMsg}";
                return new LogResult(LogLevel.Critical, msg);
            }
        }

        private static LogResult OnDbException(SqlException ex)
        {
            var dbError = GetError(ex);
            if (dbError == DatabaseError.Unhandled)
            {
                var msg = $"Unhandled DB exception. Possible connection error. {GenericExceptionMsg}";
                return new LogResult(LogLevel.Critical, msg);
            }
            else
            {
                var msg = string.Join("{0} Database error: {DbError}.", PossibleBugText);
                return new LogResult(LogLevel.Error, msg, dbError);
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

        private static DatabaseError GetError(SqlException dbException)
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
}