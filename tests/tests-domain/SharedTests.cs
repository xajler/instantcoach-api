using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Domain;

namespace Tests.Domain
{
    public class SharedTests
    {
        [Fact]
        public void Should_not_add_range_of_errors_to_validation_result_when_range_null_or_empty()
        {
            var actual = new ValidationResult();
            actual.AddError("Some error");

            actual.AddErrorRange(null);
            actual.AddErrorRange(new List<string>());
            var expected = 1;

            actual.Errors.Should().HaveCount(expected);
        }
    }
}