using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Domain;
using Core.Models;

namespace Tests.Unit
{
    public sealed class MiscTests
    {
        [Fact]
        public static void Should_not_add_range_of_errors_to_validation_result_when_range_null_or_empty()
        {
            var actual = new ValidationResult();
            actual.AddError("Some error");

            actual.AddErrorRange(null);
            actual.AddErrorRange(new List<string>());
            var expected = 1;

            actual.Errors.Should().HaveCount(expected);
        }

        [Fact]
        public static void Should_not_add_error_to_validation_result_when_range_null_or_empty()
        {
            var actual = new ValidationResult();
            actual.AddError(null);
            actual.AddError("");

            var expected = 0;

            actual.Errors.Should().HaveCount(expected);
        }

        [Fact]
        public static void Should_be_able_to_create_result_as_error()
        {
            var actual = Result.AsError(ErrorType.InvalidData);
            var expected = false;

            actual.Success.Should().Be(expected);
            actual.Error.Should().Be(ErrorType.InvalidData);
        }

        [Fact]
        public static void Test_to_cover_context_design_time_factory_in_core()
        {
            var factory = new Core.ICContextDesignTimeFactory();
            var actual = factory.CreateDbContext(new string[] { });

            actual.Should().NotBeNull();
        }
    }
}