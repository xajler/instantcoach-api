using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using static Domain.Constants.Validation;

namespace Domain
{
    public sealed class Comment : ValueObjectBase
    {
        [JsonConstructor]
        private Comment(
            CommentType commentType,
            string text,
            EvaluationCommentAuthor authorType,
            DateTime createdAt,
            int? bookmarkPinId)
        {
            CommentType = commentType;
            AuthorType = authorType;
            CreatedAt = createdAt;
            Text = text;
            BookmarkPinId = bookmarkPinId;
        }

        public CommentType CommentType { get;  }
        public string Text { get; }
        public EvaluationCommentAuthor AuthorType { get; }
        public DateTime CreatedAt { get; }
        public int? BookmarkPinId { get; }

        public Dictionary<string, IReadOnlyCollection<string>> Validate(int atIndex)
        {
            return ValidateForIndex(atIndex);
        }

        public static class Factory
        {
            public static Comment Create(CommentType commentType, string text,
                EvaluationCommentAuthor authorType, DateTime createdAt, int? bookmarkPinId)
            {
                switch (commentType)
                {
                    case CommentType.Textual:
                        return Textual(text, authorType, createdAt);
                    case CommentType.Attachment:
                        return Attachment(text, authorType, createdAt);
                    case CommentType.Bookmark:
                        return Bookmark(bookmarkPinId, authorType, createdAt);
                    default:
                        throw new ArgumentOutOfRangeException($"Unknown comment: {commentType}");
                }
            }

            public static Comment Textual(string text,
                EvaluationCommentAuthor authorType, DateTime createdAt)
            {
                return new Comment(CommentType.Textual, text, authorType, createdAt,
                    bookmarkPinId: null);
            }

            public static Comment Attachment(string text,
                EvaluationCommentAuthor authorType,
                DateTime createdAt)
            {
                return new Comment(CommentType.Attachment, text, authorType, createdAt,
                    bookmarkPinId: null);
            }

            public static Comment Bookmark(int? bookmarkPinId,
                 EvaluationCommentAuthor authorType,
                 DateTime createdAt)
            {
                return new Comment(CommentType.Bookmark, text: null, authorType, createdAt,
                    bookmarkPinId);
            }
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return CreatedAt;
            yield return CommentType;
            yield return AuthorType;
        }

        private Dictionary<string, IReadOnlyCollection<string>> ValidateForIndex(int atIndex)
        {
            var result = new Dictionary<string, IReadOnlyCollection<string>>();
            var textErrors = new List<string>();
            switch (CommentType)
            {
                case CommentType.Textual:
                    if (string.IsNullOrWhiteSpace(Text))
                    {
                        textErrors.Add("Requires a value for Textual comment.");
                    }
                    break;
                case CommentType.Attachment:
                    if (string.IsNullOrWhiteSpace(Text))
                    {
                        textErrors.Add("Requires a value for Attachment comment.");
                    }
                    if (!string.IsNullOrWhiteSpace(Text) && !Text.Contains("http"))
                    {
                        textErrors.Add("Should be a valid URL link for Attachment comment.");
                    }
                    break;
                case CommentType.Bookmark:
                    if (BookmarkPinId <= 0)
                    {
                        result.Add(FullMemberName("BookmarkPinId", atIndex),
                            new List<string> { GreaterThanZeroMsg });
                    }
                    break;
                default:
                    var msg = $"Unknown comment: {CommentType} at index [{atIndex}]";
                    result.Add(FullMemberName("CommentType", atIndex), new List<string> { msg });
                    throw new ArgumentOutOfRangeException(msg);
            }

            if (textErrors.Count > 0)
            {
                result.Add(FullMemberName("Text", atIndex), textErrors);
            }

            return result;
        }

        private static string FullMemberName(string memberName, int atIndex)
        {
            return $"Comments[{atIndex}].{memberName}";
        }
    }
}