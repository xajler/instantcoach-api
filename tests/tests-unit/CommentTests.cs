using System;
using FluentAssertions;
using Xunit;
using Domain;
using static Domain.Comment;

namespace Tests.Unit
{
    public sealed class CommentTests
    {
        private const string TextValue = "Some text value";
        private const string UrlTextValue = "https://xxx.xxx/xxx.xml";
        private const int BookmarkPinIdValue = 1;
        private static readonly EvaluationCommentAuthor AuthorTypeValue = EvaluationCommentAuthor.Agent;

        [Fact]
        public static void Should_be_of_value_object_type()
        {
            var actual = typeof(Comment);
            var expected = typeof(ValueObject);

            actual.Should().BeDerivedFrom(expected);
        }

        [Fact]
        public static void Should_be_equal_when_same_structure()
        {
            var createdAt = DateTime.UtcNow;
            var actual = Textual(TextValue, AuthorTypeValue, createdAt);
            var expected = Textual(TextValue, AuthorTypeValue, createdAt);
            actual.Should().BeEquivalentTo(expected);
            actual.GetHashCode().Should().Be(expected.GetHashCode());
            actual.Should().Be(expected);
        }

        [Fact]
        public static void Should_not_be_equal_when_same_structure()
        {
            var createdAt = DateTime.UtcNow;
            var actual = Textual(TextValue, AuthorTypeValue, DateTime.UtcNow);
            var expected = Textual(TextValue, AuthorTypeValue, createdAt);
            actual.GetHashCode().Should().NotBe(expected.GetHashCode());
            actual.Should().NotBe(expected);
        }

        [Fact]
        public static void Should_be_valid_textual_comment_via_ctor()
        {
            var comment = Textual(TextValue, AuthorTypeValue, DateTime.UtcNow);

            var actual = comment.Validate();

            actual.Should().HaveCount(0);
        }

        [Fact]
        public static void Should_be_valid_attachment_comment_via_ctor()
        {
            var comment = Attachment(UrlTextValue, AuthorTypeValue, DateTime.UtcNow);

            var actual = comment.Validate();

            actual.Should().HaveCount(0);
        }

        [Fact]
        public static void Should_be_valid_bookmark_comment_via_ctor()
        {
            var comment = Bookmark(BookmarkPinIdValue, AuthorTypeValue, DateTime.UtcNow);

            var actual = comment.Validate();

            actual.Should().HaveCount(0);
        }

        [Fact]
        public static void Should_have_errors_when_textual_comment_have_text_null_or_empty_via_ctor()
        {
            var comment = Textual(null, AuthorTypeValue, DateTime.UtcNow);

            var actual = comment.Validate();
            var expected = "Comment Text is required for Textual comment.";

            actual.Should().HaveCount(1);
            actual[0].Should().Be(expected);
        }

        [Fact]
        public static void Should_have_errors_when_attachment_comment_have_text_null_or_empty_via_ctor()
        {
            var comment = Attachment("", AuthorTypeValue, DateTime.UtcNow);

            var actual = comment.Validate();
            var expected = "Comment Text is required for Attachment comment.";

            actual.Should().HaveCount(1);
            actual[0].Should().Be(expected);
        }

        [Fact]
        public static void Should_have_errors_when_attachment_comment_have_text_without_url_via_ctor()
        {
            var comment = Attachment(TextValue, AuthorTypeValue, DateTime.UtcNow);

            var actual = comment.Validate();
            var expected = "Comment Text must be a valid URL link for Attachment comment.";

            actual.Should().HaveCount(1);
            actual[0].Should().Be(expected);
        }
    }
}