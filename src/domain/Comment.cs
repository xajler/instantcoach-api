using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Domain
{
    public sealed class Comment : ValueObject
    {
        private readonly List<string> _errors = new List<string>();

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

        public IReadOnlyList<string> Validate(int atIndex)
        {
            switch (CommentType)
            {
                case CommentType.Textual:
                    if (string.IsNullOrWhiteSpace(Text))
                    {
                        _errors.Add($"Comment [{atIndex}] Text is required for Textual comment.");
                    }
                    break;
                case CommentType.Attachment:
                    if (string.IsNullOrWhiteSpace(Text))
                    {
                        _errors.Add($"Comment [{atIndex}] Text is required for Attachment comment.");
                    }
                    if (!string.IsNullOrWhiteSpace(Text) && !Text.Contains("http"))
                    {
                        _errors.Add($"Comment [{atIndex}] Text must be a valid URL link for Attachment comment.");
                    }
                    break;
                case CommentType.Bookmark:
                    if (BookmarkPinId <= 0) { _errors.Add($"Comment [{atIndex}] BookmarkPinId should be greater than 0."); }
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Unknown comment: {CommentType} at index [{atIndex}]");
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

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return CreatedAt;
            yield return CommentType;
            yield return AuthorType;
        }
    }
}