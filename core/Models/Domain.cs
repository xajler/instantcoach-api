using System;
using System.Collections.Generic;
using static System.Console;
using static Core.Helpers;

namespace Core.Models
{
    public class InstantCoach : AggregateRoot, IAuditable
    {
        private const string CommentsContractMsg = "Comments are required";
        public InstantCoach(
            string description,
            string ticketId,
            int evaluatorId,
            int agentId,
            string evaluatorName,
            string agentName,
            int commentsCount,
            List<Comment> comments,
            List<BookmarkPin> bookmarkPins)
        {
            NotNullOrEmpty(comments, CommentsContractMsg);
            Description = description;
            // TODO: It would be nice to get this one from Config
            Status = InstantCoachStatus.New;
            TicketId = ticketId;
            Reference = CreateReference();
            EvaluatorId = evaluatorId;
            AgentId = agentId;
            EvaluatorName = evaluatorName;
            AgentName = agentName;
            Comments = comments;
            BookmarkPins = bookmarkPins;
            CommentsCount = commentsCount;
        }

        public string Description { get; }
        public InstantCoachStatus Status { get; private set; }
        public string TicketId { get; }
        public string Reference { get; private set; }
        public int EvaluatorId { get; }
        public int AgentId { get; }
        public string EvaluatorName { get; }
        public string AgentName { get; }
        public List<Comment> Comments { get; private set; }
        public List<BookmarkPin> BookmarkPins { get; private set; }
        public int CommentsCount { get; private set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public void Update(int id, UpdateType updateType,
            List<Comment> comments, List<BookmarkPin> bookmarkPins)
        {
            NotNullOrEmpty(comments, CommentsContractMsg);
            UpdateId(id);
            Status = SetStatus(updateType);
            Comments = comments;
            BookmarkPins = bookmarkPins;
        }

        public ValidationResult Validate()
        {
            var result = new ValidationResult();
            result.AddErrorRange(CreateValidationErrors());
            return result;
        }

        private InstantCoachStatus SetStatus(UpdateType updateType)
        {
            if (updateType == UpdateType.Save)
            {
                return InstantCoachStatus.InProgress;
            }
            return InstantCoachStatus.Waiting;
        }

        private List<string> CreateValidationErrors()
        {
            return new List<string>
                {
                    Description.CheckForNull(nameof(Description)),
                    Description.CheckLength(nameof(Description), 1000),
                    Description.CheckForNull(nameof(Reference)),
                    Description.CheckExactLength(nameof(Reference), 16),
                    Description.CheckForNull(nameof(TicketId)),
                    Description.CheckLength(nameof(TicketId), 64),
                    Description.CheckForNull(nameof(EvaluatorName)),
                    Description.CheckLength(nameof(EvaluatorName), 128),
                    Description.CheckForNull(nameof(AgentName)),
                    Description.CheckLength(nameof(AgentName), 128),
                }.ClenupNullItems();
        }

        private string CreateReference()
        {
            string value = DateTime.UtcNow.Ticks.ToString().Substring(5);
            return $"IC{value}";
        }
    }

    public class Range : ValueObject
    {
        public int Start { get; private set; }
        public int End { get; private set; }

        public ValidationResult Validate()
        {
            var result = new ValidationResult();
            if (End >= Start)
                result.AddError("Range start number must be greater than end number.");
            return result;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Start;
            yield return End;
        }
    }

    public class BookmarkPin : ValueObject
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public Range Range { get; set; }
        public string Comment { get; set; }
        public string MediaUrl { get; set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Id;
            yield return Index;
            yield return Range;
            yield return MediaUrl;
        }
    }

    public class Comment : ValueObject
    {
        private Comment(CommentType type, string text,
            EvaluationCommentAuthor authorType,
            DateTime createdAt)
        {
            WriteLine("inside Comment, and text value is: {0}", text);
            if (type == CommentType.Textual)
            {
                NotNullOrEmpty(text, "Text is required for Textual comment.");
            }
            else
            {
                NotEmptyOrContains(text, "http", "Text is required and must be a valid URL link for Attachment comment.");
            }

            AuthorType = authorType;
            CreatedAt = createdAt;
            Text = text;
        }

        private Comment(int bookmarkPinId,
            EvaluationCommentAuthor authorType,
            DateTime createdAt)
        {
            GreaterThanZero(bookmarkPinId, "Bookmark Pin Id must have value greter than 0.");
            CommentType = CommentType.Bookmark;
            AuthorType = authorType;
            CreatedAt = createdAt;
            BookmarkPinId = bookmarkPinId;
        }

        public CommentType CommentType { get;  }
        public string Text { get; }
        public EvaluationCommentAuthor AuthorType { get; }
        public DateTime CreatedAt { get; }
        public int BookmarkPinId { get; }

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

        public static Comment Bookmark(int bookmarkPinId,
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