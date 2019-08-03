using System;
using System.Linq;
using System.Collections.Generic;
using static Domain.Helpers;

namespace Domain
{
    public sealed partial class InstantCoach : AggregateRoot, IAuditable
    {
        private const string CommentsErrorMsg = "Comments are required to have at least one element.";
        private readonly ValidationResult _validationResult = new ValidationResult();

        public InstantCoach(
            string description,
            string ticketId,
            int evaluatorId,
            int agentId,
            string evaluatorName,
            string agentName)
        {
            Description = description;
            Status = InstantCoachStatus.New;
            TicketId = ticketId;
            Reference = CreateReference();
            EvaluatorId = evaluatorId;
            AgentId = agentId;
            EvaluatorName = evaluatorName;
            AgentName = agentName;
            CommentsCount = 0;
            Comments = new List<Comment>();
            BookmarkPins = new List<BookmarkPin>();
        }

        public string Description { get; }
        public InstantCoachStatus Status { get; private set; }
        public string TicketId { get; }
        public string Reference { get; private set; }
        public int EvaluatorId { get; }
        public int AgentId { get; }
        public string EvaluatorName { get; }
        public string AgentName { get; }
        private List<Comment> _comments;

        public InstantCoach Update(UpdateType updateType, List<Comment> comments)
        {
            return Update(updateType, comments, new List<BookmarkPin>());
        }

        // TODO: Guard those with status completed, they cannot be updated.
        public InstantCoach Update(UpdateType updateType,
            List<Comment> comments, List<BookmarkPin> bookmarkPins)
        {
            Status = SetStatus(updateType);
            ClearValues(); // EF Hack
            AddComments(comments);
            AddBookmarkPins(bookmarkPins);
            return this;
        }

        // TODO: Guard those with status New, they cannot be completed
        public InstantCoach UpdateAsCompleted()
        {
            Status = InstantCoachStatus.Completed;
            return this;
        }

        public ValidationResult Validate()
        {
            CreateValidationErrors();
            return _validationResult;
        }

        public void AddComments(List<Comment> comments)
        {

            if (comments != null && comments.Count > 0)
            {
                Comments = new List<Comment>();
                int index = 0;

                foreach (var item in comments)
                {
                    Comment comment = Comment.Create(item.CommentType, item.Text,
                        item.AuthorType, item.CreatedAt, item.BookmarkPinId);
                    var errors = comment.Validate(index);
                    if (errors.Any()) { _validationResult.Errors.AddRange(errors); }
                    else { Comments.Add(comment); }
                    index++;
                }

                CommentsCount = Comments.Count;
                // EF Hack
                CommentsValue = ToJson(Comments);
            }
            else { Comments = new List<Comment>(); }
        }

        public void AddBookmarkPins(List<BookmarkPin> bookmarkPins)
        {
            if (bookmarkPins != null && bookmarkPins.Count > 0)
            {
                BookmarkPins = new List<BookmarkPin>();
                int index = 0;
                foreach(var item in bookmarkPins)
                {
                    var pin = new BookmarkPin(
                        item.Id,
                        item.Index,
                        item.Range,
                        item.MediaUrl,
                        item.Comment);
                    var errors = pin.Validate(atIndex: index);
                    if (errors.Any()) { _validationResult.Errors.AddRange(errors); }
                    else { BookmarkPins.Add(pin); }
                    index++;
                }

                // EF Hack
                BookmarkPinsValue = ToJson(BookmarkPins);
            }
            else { BookmarkPins = new List<BookmarkPin>(); }
        }

        private static InstantCoachStatus SetStatus(UpdateType updateType)
        {
            if (updateType == UpdateType.Save)
            {
                return InstantCoachStatus.InProgress;
            }

            return InstantCoachStatus.Waiting;
        }

        private static string CreateReference()
        {
            string value = GetTicksExcludingFirst5Digits();
            return $"IC{value}";
        }

        private void CreateValidationErrors()
        {
            var descErrors = new List<string>
                {
                    Description.CheckForNull(),
                    Description.CheckLength(1000)
                }.CleanUpNullItems();
            var ticketErrors = new List<string>
                {
                    TicketId.CheckForNull(),
                    TicketId.CheckLength(64)
                }.CleanUpNullItems();
            var evalErrors = new List<string>
                {
                    EvaluatorId.CheckGreaterThanZero()
                }.CleanUpNullItems();
            var agentErrors = new List<string>
                {
                    AgentId.CheckGreaterThanZero()
                }.CleanUpNullItems();
            var evalNameErrors = new List<string>
                {
                    EvaluatorName.CheckForNull(),
                    EvaluatorName.CheckLength(128)
                }.CleanUpNullItems();
            var agentNameErrors = new List<string>
                {
                    AgentName.CheckForNull(),
                    AgentName.CheckLength(128)
                }.CleanUpNullItems();

            AddToValidationResult(nameof(Description), descErrors);
            AddToValidationResult(nameof(TicketId), ticketErrors);
            AddToValidationResult(nameof(EvaluatorId), evalErrors);
            AddToValidationResult(nameof(AgentId), agentErrors);
            AddToValidationResult(nameof(EvaluatorName), evalNameErrors);
            AddToValidationResult(nameof(AgentName), agentNameErrors);

            // Hackish second condition
            if (Comments.Count <= 0 &&
                !_validationResult.Errors.Select(x =>
                    x.Key.Contains("Comments")).Any())
            {
                _validationResult.AddError("Comments", new List<string> { CommentsErrorMsg });
            }
        }

        private void AddToValidationResult(string memberName, List<string> errors)
        {
            if (errors.Count > 0)
            {
                _validationResult.Errors.Add(memberName, errors);
            }
        }
    }

    // For EF hacks
    public partial class InstantCoach
    {
        // EF stuff and hacks
        public List<Comment> Comments
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CommentsValue))
                {
                    _comments = FromJson<List<Comment>>(CommentsValue);
                }
                return _comments;
            }
            private set => _comments = value;

        }
        private List<BookmarkPin> _bookmarkPins;
        public List<BookmarkPin> BookmarkPins
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(BookmarkPinsValue))
                {
                    _bookmarkPins = FromJson<List<BookmarkPin>>(BookmarkPinsValue);
                }
                return _bookmarkPins;
            }
            private set => _bookmarkPins = value;
        }
        public int CommentsCount { get; private set; }
        public string CommentsValue { get; private set; }
        public string BookmarkPinsValue { get; private set; }

        // Not really hack, but set via EF, and not handled by Domain.
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        private void ClearValues()
        {
            CommentsValue = null;
            BookmarkPinsValue = null;
        }
        // End EF stuff and hacks
    }
}