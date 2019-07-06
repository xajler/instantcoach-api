using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Core.Models
{
    public class InstantCoach : Entity
    {
        private const string CommentsContractMsg = "Comments are required";
        public InstantCoach(
            string description,
            string ticketId,
            int evaluatorId,
            int agentId,
            string evaluatorName,
            string agentName,
            List<Comment> comments,
            List<BookmarkPin> bookmarkPins)
        {
            Contract.Requires(comments != null, CommentsContractMsg);
            Id = 0;
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
        }

        public string Description { get; }
        public InstantCoachStatus Status { get; private set; }
        public string TicketId { get; }
        public string Reference { get; }
        public int EvaluatorId { get; }
        public int AgentId { get; }
        public string EvaluatorName { get; }
        public string AgentName { get; }
        public List<Comment> Comments { get; private set; }
        public List<BookmarkPin> BookmarkPins { get; private set; }
        public int CommentsCount => Comments.Count;

        public void Update(int id, UpdateType updateType,
            List<Comment> comments, List<BookmarkPin> bookmarkPins)
        {
            Contract.Requires(comments != null, CommentsContractMsg);
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
            return new List<String>
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

    public class BookmarkPin : Entity
    {
        public int Index { get; set; }
        public Range Range { get; set; }
        public string Comment { get; set; }
        public string MediaUrl { get; set; }
    }

    public class Comment : ValueObject
    {
        public CommentType CommentType { get; private set; }
        public string Text { get; private set; }
        public EvaluationCommentAuthor AuthorType { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public int PinId { get; private set; }

        public void AsText(string text,
            EvaluationCommentAuthor authorType,
            DateTime createdAt)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(text));
            CommentType = CommentType.Text;
            AuthorType = authorType;
            CreatedAt = createdAt;
            Text = text;
        }

        public void AsAttachment(string text,
            EvaluationCommentAuthor authorType,
            DateTime createdAt)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(text));
            CommentType = CommentType.Attachment;
            AuthorType = authorType;
            CreatedAt = createdAt;
            Text = text;
        }

        public void AsBookmark(int pinId,
            EvaluationCommentAuthor authorType,
            DateTime createdAt)
        {
            CommentType = CommentType.Bookmark;
            AuthorType = authorType;
            CreatedAt = createdAt;
            PinId = pinId;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return CreatedAt;
            yield return CommentType;
            yield return AuthorType;
        }
    }
}