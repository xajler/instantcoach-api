using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using static Domain.Constants.Validation;

namespace Domain
{
    public sealed class Comment : ValueObject
    {
        private readonly Dictionary<string, IReadOnlyCollection<string>> _errors
            = new Dictionary<string, IReadOnlyCollection<string>>();

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

        private Comment(
            CommentType type,
            string text,
            EvaluationCommentAuthor authorType,
            DateTime createdAt)
        {
            CommentType = type;
            AuthorType = authorType;
            CreatedAt = createdAt;
            Text = text;
        }

        private Comment(int? bookmarkPinId,
            EvaluationCommentAuthor authorType,
            DateTime createdAt)
        {
            CommentType = CommentType.Bookmark;
            AuthorType = authorType;
            CreatedAt = createdAt;
            BookmarkPinId = bookmarkPinId;
        }

        public CommentType CommentType { get;  }
        public string Text { get; }
        public EvaluationCommentAuthor AuthorType { get; }
        public DateTime CreatedAt { get; }
        public int? BookmarkPinId { get; }

        public Dictionary<string, IReadOnlyCollection<string>> Validate(int atIndex)
        {
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
                    if (BookmarkPinId <= 0) {
                        _errors.Add(FullMemberName("BookmarkPinId", atIndex),
                            new List<string>{ GreaterThanZeroMsg });
                    }
                    break;
                default:
                    var msg = $"Unknown comment: {CommentType} at index [{atIndex}]";
                    _errors.Add(FullMemberName("CommentType", atIndex), new List<string>{ msg });
                    throw new ArgumentOutOfRangeException(msg);
            }

            if (textErrors.Count > 0)
            {
                _errors.Add(FullMemberName("Text", atIndex), textErrors);
            }

            return _errors;
        }

        public static Comment Textual(string text,
            EvaluationCommentAuthor authorType,
            DateTime createdAt)
        {
            return new Comment(CommentType.Textual, text, authorType, createdAt);
        }

        public static Comment Attachment(string text,
            EvaluationCommentAuthor authorType,
            DateTime createdAt)
        {
            return new Comment(CommentType.Attachment, text, authorType, createdAt);
        }

        public static Comment Bookmark(int? bookmarkPinId,
             EvaluationCommentAuthor authorType,
             DateTime createdAt)
        {
            return new Comment(bookmarkPinId, authorType, createdAt);
        }

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

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return CreatedAt;
            yield return CommentType;
            yield return AuthorType;
        }

        private static string FullMemberName(string memberName, int atIndex)
        {
            return $"Comments[{atIndex}].{memberName}";
        }
    }
}