using System;
using System.Linq;
using FluentAssertions;
using Xunit;
using Domain;
using static Domain.Comment;
using static Domain.Constants.Validation;
using static Tests.Unit.TestHelpers;

namespace Tests.Unit
{
    public sealed class CommentTests
    {
        private const string TextValue = "Some text value";
        private const string UrlTextValue = "https://xxx.xxx/xxx.xml";
        private const int BookmarkPinIdValue = 1;
        private static readonly EvaluationCommentAuthor AuthorTypeValue =
            EvaluationCommentAuthor.Agent;

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

            var actual = NewTextualComment(createdAt);
            var expected = NewTextualComment(createdAt);

            actual.Should().BeEquivalentTo(expected);
            actual.GetHashCode().Should().Be(expected.GetHashCode());
            actual.Should().Be(expected);
        }

        [Fact]
        public static void Should_not_be_equal_when_same_structure()
        {
            var actual = NewTextualComment();
            var expected = NewTextualComment();
            actual.GetHashCode().Should().NotBe(expected.GetHashCode());
            actual.Should().NotBe(expected);
        }

        [Fact]
        public static void Should_be_valid_textual_comment_via_ctor()
        {
            var sut = NewTextualComment();

            var actual = sut.Validate(atIndex: 0);

            actual.Should().HaveCount(0);
        }

        [Fact]
        public static void Should_be_valid_attachment_comment_via_ctor()
        {
            var sut = NewAttachmentComment();

            var actual = sut.Validate(atIndex: 0);

            actual.Should().HaveCount(0);
        }

        [Fact]
        public static void Should_be_valid_bookmark_comment_via_ctor()
        {
            var sut = NewBookmarkComment();

            var actual = sut.Validate(atIndex: 4);

            actual.Should().HaveCount(0);
        }

        [Fact]
        public static void Should_have_errors_when_textual_comment_have_text_null_or_empty_via_ctor()
        {
            var sut = Textual(null, AuthorTypeValue, DateTime.UtcNow);

            RunAsserts(sut, atIndex: 0, "Text",
                "Requires a value for Textual comment.");
        }

        [Fact]
        public static void Should_have_errors_when_attachment_comment_have_text_null_or_empty_via_ctor()
        {
            var sut = Attachment("", AuthorTypeValue, DateTime.UtcNow);

            RunAsserts(sut, atIndex: 2, "Text",
                "Requires a value for Attachment comment.");
        }

        [Fact]
        public static void Should_have_errors_when_attachment_comment_have_text_without_url_via_ctor()
        {
            var sut = Attachment(TextValue, AuthorTypeValue, DateTime.UtcNow);
            RunAsserts(sut, atIndex: 1, "Text",
                "Should be a valid URL link for Attachment comment.");
        }

        [Fact]
        public static void Should_have_errors_when_bookmark_have_pin_id_zero_or_less_via_ctor()
        {
            var sut = Bookmark(0, AuthorTypeValue, DateTime.UtcNow);

            RunAsserts(sut, atIndex: 3, "BookmarkPinId", GreaterThanZeroMsg);
        }

        private static void RunAsserts(Comment sut, int atIndex, string memberName, string errorMsg)
        {
            var validationResult = sut.Validate(atIndex: atIndex);
            var actual = validationResult.First();
            var (expectedMember, expectedErrs) = CreateExpectedValues(
                "Comments", memberName, atIndex: atIndex, errorMsg);

            actual.Value.Should().HaveCount(expectedErrs.Count);
            actual.Key.Should().Contain(expectedMember);
            actual.Value.First().Should().Be(expectedErrs.First());
        }

        private static Comment NewTextualComment()
        {
            return NewTextualComment(createdAt: DateTime.UtcNow);
        }

        private static Comment NewTextualComment(DateTime createdAt)
        {
            return Textual(TextValue, AuthorTypeValue, createdAt);
        }

        private static Comment NewAttachmentComment()
        {
            return Attachment(UrlTextValue, AuthorTypeValue, DateTime.UtcNow);
        }

        private static Comment NewBookmarkComment()
        {
            return Bookmark(BookmarkPinIdValue, AuthorTypeValue, DateTime.UtcNow);
        }
    }
}