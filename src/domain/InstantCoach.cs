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

        public ValidationResult UpdateAndValidate(UpdateType updateType,
            List<Comment> comments, List<BookmarkPin> bookmarkPins, int id = 0)
        {
            UpdateId(id);
            Status = SetStatus(updateType);
            AddComments(comments);
            AddBookmarkPins(bookmarkPins);
            return Validate();
        }

        public ValidationResult UpdateAsCompletedAndValidate()
            => UpdateAsCompletedAndValidate(id: 0);


        public ValidationResult UpdateAsCompletedAndValidate(int id)
        {
            UpdateId(id);
            Status = InstantCoachStatus.Completed;
            return Validate();
        }

        public ValidationResult Validate()
        {
            CreateValidationErrors();
            return _validationResult;
        }

        // TODO: Must be called when null.
        //       Needs better solution or Factory.
        public void AddComments(List<Comment> comments)
        {
            if (comments != null && comments.Count > 0)
            {
                Comments = new List<Comment>();
                int index = 0;

                foreach (var item in comments)
                {
                    Comment comment = CreateComment(item);
                    var errors = comment.Validate(index);
                    if (errors.Any()) { _validationResult.Errors.AddRange(errors); }
                    else { Comments.Add(comment); }
                    index++;
                }

                CommentsCount = Comments.Count;
                // EF Hack
                CommentsConvert = Comments;
            }
            else
            {
                _validationResult.AddError("Comments", new List<string> { CommentsErrorMsg });
            }
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
            }
            // EF Hack
            BookmarkPinsConvert = BookmarkPins;
        }

        private static InstantCoachStatus SetStatus(UpdateType updateType)
        {
            if (updateType == UpdateType.Save)
            {
                return InstantCoachStatus.InProgress;
            }

            return InstantCoachStatus.Waiting;
        }

        private static Comment CreateComment(Comment item)
        {
            switch (item.CommentType)
            {
                case CommentType.Textual:
                    return Comment.Textual(
                        item.Text, item.AuthorType, item.CreatedAt);
                case CommentType.Attachment:
                    return Comment.Attachment(
                        item.Text, item.AuthorType, item.CreatedAt);
                case CommentType.Bookmark:
                    return Comment.Bookmark(
                        item.BookmarkPinId, item.AuthorType, item.CreatedAt);
                default:
                    throw new ArgumentOutOfRangeException($"Unknown comment: {item.CommentType}");
            }
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
    public sealed partial class InstantCoach
    {
        // EF stuff and hacks
        public string CommentsValue { get; private set; }
        public string BookmarkPinsValue { get; private set; }
        public List<Comment> CommentsConvert
        {
            get { return new List<Comment>(); }
            private set { CommentsValue = ToJson(value); }
        }
        public List<BookmarkPin> BookmarkPinsConvert
        {
            get { return new List<BookmarkPin>(); }
            private set
            {
                if (value != null && value.Count > 0)
                {
                    BookmarkPinsValue = ToJson(value);
                }
                else { BookmarkPinsValue = null; }
            }
        }

        // Not really hack, but set via EF, and not handled by Domain.
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        // End EF stuff and hacks
    }
}