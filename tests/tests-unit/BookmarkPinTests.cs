using FluentAssertions;
using Xunit;
using Domain;

namespace Tests.Unit
{
    public class BookmarkPinTests
    {
        private const string MediaUrlValue = "https://example.com/test.png";
        private const string CommentValue =  "No comment";

        [Fact]
        public void Should_be_of_value_object_type()
        {
            var actual = typeof(BookmarkPin);
            var expected = typeof(ValueObject);

            actual.Should().BeDerivedFrom(expected);
        }

        [Fact]
        public void Should_be_valid_bookmark_pin_via_ctor()
        {
            var bp = new BookmarkPin(
                id: 1,
                index: 1,
                new Range(1, 2),
                MediaUrlValue,
                CommentValue);

            var actual = bp.Validate();

            actual.Should().HaveCount(0);
        }

        [Fact]
        public void Should_be_equal_when_same_structure()
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
        public void Should_not_be_equal_when_same_structure()
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
        public void Should_be_valid_bookmark_witouth_comment_pin_via_ctor()
        {
            var bp = new BookmarkPin(
                id: 1,
                index: 1,
                new Range(1, 2),
                MediaUrlValue,
                comment: null);

            var actual = bp.Validate();

            actual.Should().HaveCount(0);
        }

        [Fact]
        public void Should_have_errors_when_id_smaller_or_equal_zero_via_ctor()
        {
            var bp = new BookmarkPin(
                id: 0,
                index: 1,
                new Range(1, 2),
                MediaUrlValue,
                CommentValue);

            var actual = bp.Validate();
            var expected = "Bookmark Pin Id should be greater than 0.";

            actual.Should().HaveCount(1);
            actual.Should().Contain(expected);
        }

        [Fact]
        public void Should_have_errors_when_index_smaller_or_equal_zero_via_ctor()
        {
            var bp = new BookmarkPin(
                id: 1,
                index: 0,
                new Range(1, 2),
                MediaUrlValue,
                CommentValue);

            var actual = bp.Validate();
            var expected = "Bookmark Pin Index should be greater than 0.";

            actual.Should().HaveCount(1);
            actual.Should().Contain(expected);
        }

        [Fact]
        public void Should_have_errors_when_range_start_smaller_or_equal_zero_via_ctor()
        {
            var bp = new BookmarkPin(
                id: 1,
                index: 1,
                new Range(0, 2),
                MediaUrlValue,
                CommentValue);

            var actual = bp.Validate();
            var expected = "Bookmark Pin Range Start should be greater than 0.";

            actual.Should().HaveCount(1);
            actual.Should().Contain(expected);
        }

        [Fact]
        public void Should_have_errors_when_range_start_should_be_greater_then_end_via_ctor()
        {
            var bp = new BookmarkPin(
                id: 1,
                index: 1,
                new Range(2, 1),
                MediaUrlValue,
                CommentValue);

            var actual = bp.Validate();
            var expected = "Bookmark Pin Range end number must be greater than start number.";

            actual.Should().HaveCount(1);
            actual.Should().Contain(expected);
        }

        [Fact]
        public void Should_have_errors_when_media_url_is_null_or_empty_via_ctor()
        {
            var bp = new BookmarkPin(
                id: 1,
                index: 1,
                new Range(1, 2),
                null,
                CommentValue);

            var actual = bp.Validate();
            var expected = "Bookmark Pin MediaUrl is required.";

            actual.Should().HaveCount(1);
            actual.Should().Contain(expected);
        }

        // foreach (var error in actual)
        // {
        //     WriteLine(error);
        // }
    }
}