using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Domain;
using static Domain.Constants.Validation;

namespace Tests.Unit
{
    public sealed class BookmarkPinTests
    {
        private const string MediaUrlValue = "https://example.com/test.png";
        private const string CommentValue =  "No comment";

        [Fact]
        public static void Should_be_of_value_object_type()
        {
            var actual = typeof(BookmarkPin);
            var expected = typeof(ValueObject);

            actual.Should().BeDerivedFrom(expected);
        }

        [Fact]
        public static void Should_be_valid_bookmark_pin_via_ctor_without_comment()
        {
            var bp = new BookmarkPin(
                id: 1,
                index: 1,
                new Range(1, 2),
                MediaUrlValue);

            var actual = bp.Validate(atIndex: 0);

            actual.Should().HaveCount(0);
        }

        [Fact]
        public static void Should_be_valid_bookmark_pin_via_ctor_full()
        {
            var bp = new BookmarkPin(
                id: 1,
                index: 1,
                new Range(1, 2),
                MediaUrlValue,
                CommentValue);

            var actual = bp.Validate(atIndex: 0);

            actual.Should().HaveCount(0);
        }

        [Fact]
        public static void Should_be_equal_when_same_structure()
        {
            var actual = new BookmarkPin(
                id: 1,
                index: 1,
                new Range(1, 2),
                MediaUrlValue,
                CommentValue);

            var expected = new BookmarkPin(
                id: 1,
                index: 1,
                new Range(1, 2),
                MediaUrlValue,
                CommentValue);

            actual.Should().BeEquivalentTo(expected);
            actual.GetHashCode().Should().Be(expected.GetHashCode());
        }

        [Fact]
        public static void Should_not_be_equal_when_same_structure()
        {
            var actual = new BookmarkPin(
                id: 1,
                index: 1,
                new Range(1, 2),
                MediaUrlValue,
                CommentValue);

            var expected = new BookmarkPin(
                id: 1,
                index: 2,
                new Range(1, 2),
                MediaUrlValue,
                CommentValue);

            actual.Should().NotBeEquivalentTo(expected);
        }

        [Fact]
        public static void Should_be_valid_bookmark_without_comment_pin_via_ctor()
        {
            var bp = new BookmarkPin(
                id: 1,
                index: 1,
                new Range(1, 2),
                MediaUrlValue,
                comment: null);

            var actual = bp.Validate(atIndex: 0);

            actual.Should().HaveCount(0);
        }

        [Fact]
        public static void Should_have_errors_when_id_smaller_or_equal_zero_via_ctor()
        {
            var bp = new BookmarkPin(
                id: 0,
                index: 1,
                new Range(1, 2),
                MediaUrlValue,
                CommentValue);

            var validationResult = bp.Validate(atIndex: 0);
            var actual = validationResult.First();
            var (expectedMember, expectedErrs) = CreateExpectedValues(
                "Id", atIndex: 0, GreaterThanZeroMsg);

            actual.Value.Should().HaveCount(expectedErrs.Count);
            actual.Key.Should().Contain(expectedMember);
            actual.Value.First().Should().Be(expectedErrs.First());
        }

        [Fact]
        public static void Should_have_errors_when_index_smaller_or_equal_zero_via_ctor()
        {
            var bp = new BookmarkPin(
                id: 1,
                index: 0,
                new Range(1, 2),
                MediaUrlValue,
                CommentValue);

            var validationResult = bp.Validate(atIndex: 1);
            var actual = validationResult.First();
            var (expectedMember, expectedErrs) = CreateExpectedValues(
                "Index", atIndex: 1, GreaterThanZeroMsg);

            actual.Value.Should().HaveCount(expectedErrs.Count);
            actual.Key.Should().Contain(expectedMember);
            actual.Value.First().Should().Be(expectedErrs.First());
        }

        [Fact]
        public static void Should_have_errors_when_range_start_smaller_or_equal_zero_via_ctor()
        {
            var bp = new BookmarkPin(
                id: 1,
                index: 1,
                new Range(0, 2),
                MediaUrlValue,
                CommentValue);

            var validationResult = bp.Validate(atIndex: 2);
            var actual = validationResult.First();
            var (expectedMember, expectedErrs) = CreateExpectedValues(
                "Range.Start", atIndex: 2, GreaterThanZeroMsg);

            actual.Value.Should().HaveCount(expectedErrs.Count);
            actual.Key.Should().Contain(expectedMember);
            actual.Value.First().Should().Be(expectedErrs.First());
        }

        [Fact]
        public static void Should_have_errors_when_range_start_should_be_greater_then_end_via_ctor()
        {
            var bp = new BookmarkPin(
                id: 1,
                index: 1,
                new Range(2, 1),
                MediaUrlValue,
                CommentValue);

            var validationResult = bp.Validate(atIndex: 3);
            var actual = validationResult.First();
            var (expectedMember, expectedErrs) = CreateExpectedValues(
                "Range.End", atIndex: 3, "Should be greater than Range Start number.");

            actual.Value.Should().HaveCount(expectedErrs.Count);
            actual.Key.Should().Contain(expectedMember);
            actual.Value.First().Should().Be(expectedErrs.First());
        }

        [Fact]
        public static void Should_have_errors_when_media_url_is_null_or_empty_via_ctor()
        {
            var bp = new BookmarkPin(
                id: 1,
                index: 1,
                new Range(1, 2),
                null,
                CommentValue);

            var validationResult = bp.Validate(atIndex: 4);
            var actual = validationResult.First();
            var (expectedMember, expectedErrs) = CreateExpectedValues(
                "MediaUrl", atIndex: 4,  RequiredMsg);

            actual.Value.Should().HaveCount(expectedErrs.Count);
            actual.Key.Should().Contain(expectedMember);
            actual.Value.First().Should().Be(expectedErrs.First());
        }

        private static (string, IReadOnlyCollection<string>) CreateExpectedValues(
            string memberName, int atIndex, string errorText)
        {
            return ($"BookmarkPins[{atIndex}].{memberName}", new List<string> { errorText });
        }
    }
}