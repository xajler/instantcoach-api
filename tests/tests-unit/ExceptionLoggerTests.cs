using System;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Xunit;
using Core;

namespace Tests.Unit
{
    public static class ExceptionLoggerTests
    {
        private const string ExMsg = "ups";
        [Fact]
        public static void Should_be_log_level_critical_when_unhandled_exception()
        {
            var ex = new Exception(ExMsg);
            var sut = new ExceptionLogger();

            var actual = sut.GetLogResult(ex);

            actual.Level.Should().Be(LogLevel.Critical);
            actual.Message.Should().Contain("Unknown exception of Type");
            actual.IsDbError.Should().BeFalse();
        }

        [Fact]
        public static void Should_be_log_level_error_when_usual_index_out_of_range_exception()
        {
            var ex = new IndexOutOfRangeException(ExMsg);
            var sut = new ExceptionLogger();

            var actual = sut.GetLogResult(ex);

            actual.Level.Should().Be(LogLevel.Error);
            actual.Message.Should().Contain(Constants.PossibleBugText);
        }

        [Fact]
        public static void Should_be_log_level_error_when_usual_invalid_operation_exception()
        {
            var ex = new InvalidOperationException(ExMsg);
            var sut = new ExceptionLogger();

            var actual = sut.GetLogResult(ex);

            actual.Level.Should().Be(LogLevel.Error);
        }

        [Fact]
        public static void Should_be_log_level_error_when_usual_null_reference_exception()
        {
            var ex = new NullReferenceException(ExMsg);
            var sut = new ExceptionLogger();

            var actual = sut.GetLogResult(ex);

            actual.Level.Should().Be(LogLevel.Error);
        }

        [Fact]
        public static void Should_be_log_level_error_when_usual_argument_out_of_range_exception()
        {
            var ex = new ArgumentOutOfRangeException(ExMsg);
            var sut = new ExceptionLogger();

            var actual = sut.GetLogResult(ex);

            actual.Level.Should().Be(LogLevel.Error);
        }

        [Fact]
        public static void Should_be_log_level_error_when_usual_argument_null_exception()
        {
            var ex = new ArgumentNullException(ExMsg);
            var sut = new ExceptionLogger();

            var actual = sut.GetLogResult(ex);

            actual.Level.Should().Be(LogLevel.Error);
        }

        [Fact]
        public static void Should_be_log_level_error_when_usual_argument_exception()
        {
            var ex = new ArgumentException(ExMsg);
            var sut = new ExceptionLogger();

            var actual = sut.GetLogResult(ex);

            actual.Level.Should().Be(LogLevel.Error);
        }

        [Fact]
        public static void Should_be_log_level_critical_when_db_update_exception()
        {
            var ex = new DbUpdateException("db error", new Exception(ExMsg));
            var sut = new ExceptionLogger();

            var actual = sut.GetLogResult(ex);

            actual.Level.Should().Be(LogLevel.Critical);
            actual.Message.Should().Contain("Failed to Save DB Changes");
        }

        [Fact]
        public static void Should_be_log_level_critical_when_db_update_exception_inner_is_sql_exception()
        {
            var sEx = SqlExceptionCreator.New();
            var ex = new DbUpdateException(ExMsg, sEx);
            var sut = new ExceptionLogger();

            var actual = sut.GetLogResult(ex);

            actual.Level.Should().Be(LogLevel.Critical);
            actual.Message.Should().Contain("Unhandled DB exception. Possible connection error");
        }

        [Fact]
        public static void Should_be_db_error_unique_cannot_insert_null_when_sql_exception_with_number_515()
        {
            var ex = SqlExceptionCreator.New(515);
            var sut = new ExceptionLogger();

            var actual = sut.GetLogResult(ex);

            actual.Level.Should().Be(LogLevel.Error);
            actual.DbError.Should().Be(DatabaseError.CannotInsertNull);
            actual.IsDbError.Should().BeTrue();
            actual.Message.Should().Contain(Constants.PossibleBugText);
        }

        [Fact]
        public static void Should_be_db_error_unique_constraint_when_sql_exception_with_number_2601()
        {
            var ex = SqlExceptionCreator.New(2601);
            var sut = new ExceptionLogger();

            var actual = sut.GetLogResult(ex);

            actual.Level.Should().Be(LogLevel.Error);
            actual.DbError.Should().Be(DatabaseError.UniqueConstraint);
        }

        [Fact]
        public static void Should_be_db_error_unique_constraint_when_sql_exception_with_number_2627()
        {
            var ex = SqlExceptionCreator.New(2627);
            var sut = new ExceptionLogger();

            var actual = sut.GetLogResult(ex);

            actual.Level.Should().Be(LogLevel.Error);
            actual.DbError.Should().Be(DatabaseError.UniqueConstraint);
        }

        [Fact]
        public static void Should_be_db_error_numeric_overflow_when_sql_exception_with_number_8115()
        {
            var ex = SqlExceptionCreator.New(8115);
            var sut = new ExceptionLogger();

            var actual = sut.GetLogResult(ex);

            actual.Level.Should().Be(LogLevel.Error);
            actual.DbError.Should().Be(DatabaseError.NumericOverflow);
        }

        [Fact]
        public static void Should_be_db_error_max_length_when_sql_exception_with_number_8152()
        {
            var ex = SqlExceptionCreator.New(8152);
            var sut = new ExceptionLogger();

            var actual = sut.GetLogResult(ex);

            actual.Level.Should().Be(LogLevel.Error);
            actual.DbError.Should().Be(DatabaseError.MaxLength);
        }

        private static class SqlExceptionCreator
        {
            private static T Construct<T>(params object[] p)
            {
                var ctors = typeof(T).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance);
                return (T)ctors.First(ctor => ctor.GetParameters().Length == p.Length).Invoke(p);
            }

            internal static SqlException New(int number = 1)
            {
                SqlErrorCollection collection = Construct<SqlErrorCollection>();
                SqlError error = Construct<SqlError>(number, (byte)2, (byte)3, "server name", "error message", "proc", 100, null);

                typeof(SqlErrorCollection)
                    .GetMethod("Add", BindingFlags.NonPublic | BindingFlags.Instance)
                    .Invoke(collection, new object[] { error });


                return typeof(SqlException)
                    .GetMethod("CreateException", BindingFlags.NonPublic | BindingFlags.Static,
                        null,
                        CallingConventions.ExplicitThis,
                        new[] { typeof(SqlErrorCollection), typeof(string) },
                        new ParameterModifier[] { })
                    .Invoke(null, new object[] { collection, "7.0.0" }) as SqlException;
            }
        }
    }
}