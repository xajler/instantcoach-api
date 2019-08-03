using System;
using System.Collections.Generic;
using System.Linq;
using Domain;
using static Domain.Comment;

namespace Tests.Unit
{
    public sealed class TestEntity : Entity
    {
        public TestEntity(int id)
        {
            base.UpdateId(id);
        }
    }

    public static class TestHelpers
    {
        public const string DescriptionValue = "Some description";
        public const string TicketIdValue = "42";
        public const int AgentIdValue = 1;
        public const int EvaluatorIdValue = 2;
        public const string EvaluatorNameValue = "John Evaluator";
        public const string AgentNameValue = "Jane Agent";
        public const string TextValue = "Some text value";
        public const string UrlTextValue = "https://xxx.xxx/xxx.xml";
        public const int BookmarkPinIdValue = 1;
        public static readonly EvaluationCommentAuthor AuthorTypeValue =
            EvaluationCommentAuthor.Agent;
        public const string MediaUrlValue = "https://example.com/test.png";
        public const string CommentValue = "No comment";
        private static readonly Random random = new Random();

        public static string GenerateStringOfLength(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static InstantCoach NewInstantCoach()
        {
            return new InstantCoach(
                DescriptionValue,
                TicketIdValue,
                EvaluatorIdValue,
                AgentIdValue,
                EvaluatorNameValue,
                AgentNameValue);
        }

        public static InstantCoach NewInstantCoachWitComments()
        {
            InstantCoach result = NewInstantCoach();
            result.AddComments(GetComments());
            return result;
        }

        public static Comment NewTextualComment()
        {
            return NewTextualComment(createdAt: DateTime.UtcNow);
        }

        public static Comment NewTextualComment(DateTime createdAt)
        {
            return Textual(TextValue, AuthorTypeValue, createdAt);
        }

        public static Comment NewAttachmentComment()
        {
            return Attachment(UrlTextValue, AuthorTypeValue, DateTime.UtcNow);
        }

        public static Comment NewBookmarkComment()
        {
            return Bookmark(BookmarkPinIdValue, AuthorTypeValue, DateTime.UtcNow);
        }

        public static BookmarkPin NewBookmarkPinWithComment()
        {
            return new BookmarkPin(
                id: 1,
                index: 1,
                new Range(1, 2),
                MediaUrlValue,
                CommentValue);
        }

        public static List<Comment> GetComments()
        {
            return new List<Comment>
            {
                Bookmark(bookmarkPinId: 1, authorType: EvaluationCommentAuthor.Agent, createdAt: DateTime.UtcNow),
                Textual("some comment", authorType: EvaluationCommentAuthor.Evaluator, createdAt: DateTime.UtcNow)
            };
        }

        public static List<Comment> GetUpdateComments()
        {
            var result = GetComments();
            result.Add(
                Attachment(text: "http://somecomment", authorType: EvaluationCommentAuthor.Agent, createdAt: DateTime.UtcNow));
            return result;
        }

        public static List<BookmarkPin> GetBookmarkPins()
        {
            return new List<BookmarkPin>
            {
                new BookmarkPin(id: 1, index: 1, new Range(1, 2),
                    mediaurl: "https://example.com/test.png", comment: "No comment")
            };
        }

        public static List<Comment> GetInvalidComments()
        {
            return new List<Comment>
            {
                Bookmark(bookmarkPinId: 0, authorType: EvaluationCommentAuthor.Agent, createdAt: DateTime.UtcNow),
                Textual(null, authorType: EvaluationCommentAuthor.Evaluator, createdAt: DateTime.UtcNow)
            };
        }

        public static List<BookmarkPin> GetInvalidBookmarkPins()
        {
            var result = new List<BookmarkPin>();
            var bookmarkPin = new BookmarkPin(id: 0, index: 1, new Range(1, 2),
                mediaurl: "https://example.com/test.png", comment: "No comment");
            result.Add(bookmarkPin);
            return result;
        }

        public static ValidationResult CreateValidationResult(
            string memberName, string errorText)
        {
            var result = new ValidationResult();
            result.AddError($"{memberName}", new List<string> { errorText });
            return result;
        }

        public static (string, IReadOnlyCollection<string>) CreateExpectedValues(
            string prefix, string memberName, int atIndex, string errorText)
        {
            return ($"{prefix}[{atIndex}].{memberName}", new List<string> { errorText });
        }
    }
}