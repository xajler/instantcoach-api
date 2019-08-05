using System.Linq;
using FluentAssertions;
using Xunit;
using Domain;
using static Domain.Constants.Validation;
using static Tests.Unit.TestHelpers;

namespace Tests.Unit
{
    public sealed class BookmarkPinTests
    {
        private const string MediaUrlValue = "https://example.com/test.png";
        private const string CommentValue = "No comment";

        [Fact]
        public static void Should_be_of_value_object_type()
        {
            var actual = typeof(BookmarkPin);
            var expected = typeof(ValueObjectBase);

            actual.Should().BeDerivedFrom(expected);
        }

        [Fact]
        public static void Should_be_valid_bookmark_pin_via_ctor_without_comment()
        {
            var sut = NewBookmarkPin(comment: null);

            var actual = sut.Validate(atIndex: 0);

            actual.Should().HaveCount(0);
        }

        [Fact]
        public static void Should_be_valid_bookmark_pin_via_ctor_full()
        {
            var sut = NewBookmarkPin();

            var actual = sut.Validate(atIndex: 0);

            actual.Should().HaveCount(0);
        }

        [Fact]
        public static void Should_be_equal_when_same_structure()
        {
            var actual = NewBookmarkPin();
            var expected = NewBookmarkPin();

            actual.Should().BeEquivalentTo(expected);
            actual.GetHashCode().Should().Be(expected.GetHashCode());
        }

        [Fact]
        public static void Should_not_be_equal_when_not_same_structure()
        {
            var actual = NewBookmarkPin();
            var expected = BookmarkPin.Factory.Create(
                id: 1,
                index: 2,
                Range.Factory.Create(1, 2),
                MediaUrlValue,
                CommentValue);

            actual.Should().NotBeEquivalentTo(expected);
        }

        [Fact]
        public static void Should_be_valid_bookmark_without_comment_pin_via_ctor()
        {
            var sut = NewBookmarkPin(comment: null);

            var actual = sut.Validate(atIndex: 0);

            actual.Should().HaveCount(0);
        }

        [Fact]
        public static void Should_have_errors_when_id_smaller_or_equal_zero_via_ctor()
        {
            var sut = BookmarkPin.Factory.Create(
                id: 0,
                index: 1,
                Range.Factory.Create(1, 2),
                MediaUrlValue,
                comment: null);

            RunAsserts(sut, atIndex: 0, "Id", GreaterThanZeroMsg);
        }

        [Fact]
        public static void Should_have_errors_when_index_smaller_or_equal_zero_via_ctor()
        {
            var sut = BookmarkPin.Factory.Create(
                id: 1,
                index: 0,
                Range.Factory.Create(1, 2),
                MediaUrlValue,
                CommentValue);

            RunAsserts(sut, atIndex: 1, "Index", GreaterThanZeroMsg);
        }

        [Fact]
        public static void Should_have_errors_when_range_start_smaller_or_equal_zero_via_ctor()
        {
            var sut = BookmarkPin.Factory.Create(
                id: 1,
                index: 1,
                Range.Factory.Create(0, 2),
                MediaUrlValue,
                CommentValue);

            RunAsserts(sut, atIndex: 2, "Range.Start", GreaterThanZeroMsg);
        }

        [Fact]
        public static void Should_have_errors_when_range_start_should_be_greater_then_end_via_ctor()
        {
            var sut = BookmarkPin.Factory.Create(
                id: 1,
                index: 1,
                Range.Factory.Create(2, 1),
                MediaUrlValue,
                CommentValue);

            RunAsserts(sut, atIndex: 3, "Range.End",
                "Should be greater than Range Start number.");
        }

        [Fact]
        public static void Should_have_errors_when_media_url_is_null_or_empty_via_ctor()
        {
            var sut = BookmarkPin.Factory.Create(
                id: 1,
                index: 1,
                Range.Factory.Create(1, 2),
                null,
                CommentValue);

            RunAsserts(sut, atIndex: 4, "MediaUrl", RequiredMsg);
        }

        private static void RunAsserts(BookmarkPin sut, int atIndex, string memberName, string errorMsg)
        {
            var validationResult = sut.Validate(atIndex: atIndex);
            var actual = validationResult.First();
            var (expectedMember, expectedErrs) = CreateExpectedValues(
                "BookmarkPins", memberName, atIndex: atIndex, errorMsg);

            actual.Value.Should().HaveCount(expectedErrs.Count);
            actual.Key.Should().Contain(expectedMember);
            actual.Value.First().Should().Be(expectedErrs.First());
        }

        private static BookmarkPin NewBookmarkPin(string comment = CommentValue)
        {
            return BookmarkPin.Factory.Create(
                id: 1,
                index: 1,
                Range.Factory.Create(1, 2),
                MediaUrlValue,
                comment);
        }
    }
}