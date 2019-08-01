using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Domain;
using static Domain.Comment;
using static Domain.Constants.Validation;

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

            var actual = comment.Validate(atIndex: 0);

            actual.Should().HaveCount(0);
        }

        [Fact]
        public static void Should_be_valid_attachment_comment_via_ctor()
        {
            var comment = Attachment(UrlTextValue, AuthorTypeValue, DateTime.UtcNow);

            var actual = comment.Validate(atIndex: 0);

            actual.Should().HaveCount(0);
        }

        [Fact]
        public static void Should_be_valid_bookmark_comment_via_ctor()
        {
            var comment = Bookmark(BookmarkPinIdValue, AuthorTypeValue, DateTime.UtcNow);

            var actual = comment.Validate(atIndex: 4);

            actual.Should().HaveCount(0);
        }

        [Fact]
        public static void Should_have_errors_when_textual_comment_have_text_null_or_empty_via_ctor()
        {
            var comment = Textual(null, AuthorTypeValue, DateTime.UtcNow);

            var validationResult = comment.Validate(atIndex: 0);
            var actual = validationResult.First();
            var (expectedMember, expectedErrs) = CreateExpectedValues(
                "Text", atIndex: 0, "Requires a value for Textual comment.");

            actual.Value.Should().HaveCount(expectedErrs.Count);
            actual.Key.Should().Contain(expectedMember);
            actual.Value.First().Should().Be(expectedErrs.First());
        }

        [Fact]
        public static void Should_have_errors_when_attachment_comment_have_text_null_or_empty_via_ctor()
        {
            var comment = Attachment("", AuthorTypeValue, DateTime.UtcNow);

            var validationResult = comment.Validate(atIndex: 2);
            var actual = validationResult.First();
            var (expectedMember, expectedErrs) = CreateExpectedValues(
                "Text", atIndex: 2, "Requires a value for Attachment comment.");

            actual.Value.Should().HaveCount(expectedErrs.Count);
            actual.Key.Should().Contain(expectedMember);
            actual.Value.First().Should().Be(expectedErrs.First());
        }

        [Fact]
        public static void Should_have_errors_when_attachment_comment_have_text_without_url_via_ctor()
        {
            var comment = Attachment(TextValue, AuthorTypeValue, DateTime.UtcNow);

            var validationResult = comment.Validate(atIndex: 1);
            var actual = validationResult.First();
            var (expectedMember, expectedErrs) = CreateExpectedValues(
                "Text", atIndex: 1, "Should be a valid URL link for Attachment comment.");

            actual.Value.Should().HaveCount(expectedErrs.Count);
            actual.Key.Should().Contain(expectedMember);
            actual.Value.First().Should().Be(expectedErrs.First());
        }

        [Fact]
        public static void Should_have_errors_when_bookmark_have_pin_id_zero_or_less_via_ctor()
        {
            var comment = Bookmark(0, AuthorTypeValue, DateTime.UtcNow);

            var validationResult = comment.Validate(atIndex: 3);
            var actual = validationResult.First();
            var (expectedMember, expectedErrs) = CreateExpectedValues(
                "BookmarkPinId", atIndex: 3, GreaterThanZeroMsg);

            actual.Value.Should().HaveCount(expectedErrs.Count);
            actual.Key.Should().Contain(expectedMember);
            actual.Value.First().Should().Be(expectedErrs.First());
        }


        private static (string, IReadOnlyCollection<string>) CreateExpectedValues(
            string memberName, int atIndex, string errorText)
        {
            return ($"Comments[{atIndex}].{memberName}", new List<string> { errorText });
        }
    }
}