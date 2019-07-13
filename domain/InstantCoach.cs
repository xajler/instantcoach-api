using System;
using System.Linq;
using System.Collections.Generic;
using static Domain.Helpers;

namespace Domain
{
    public partial class InstantCoach : AggregateRoot, IAuditable
    {
        private const string CommentsErrorMsg = "Comments are required to have at least one element.";
        private readonly ValidationResult _errors = new ValidationResult();

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

        public ValidationResult UpdateAsCompletedAndValidate(int id = 0)
        {
            UpdateId(id);
            Status = InstantCoachStatus.Completed;
            return Validate();
        }

        public ValidationResult Validate()
        {
            _errors.AddErrorRange(CreateValidationErrors());
            return _errors;
        }

        // TODO: Must be called when null.
        //       Needs better solution or Factory.
        public void AddComments(List<Comment> comments)
        {
            if (comments != null && comments.Count > 0)
            {
                Comments = new List<Comment>();
                int index = 1;
                foreach (var item in comments)
                {
                    Comment comment = null;
                    Console.WriteLine($"[{index}] type: {item.CommentType}");
                    switch (item.CommentType)
                    {
                        case CommentType.Textual:
                            comment = Comment.Textual(item.Text, item.AuthorType, item.CreatedAt);
                            break;
                        case CommentType.Attachment:
                            comment = Comment.Attachment(item.Text, item.AuthorType, item.CreatedAt);
                            break;
                        case CommentType.Bookmark:
                            //Console.WriteLine($"[{index}] BKMID: {item.BookmarkPinId}");
                            comment = Comment.Bookmark(item.BookmarkPinId, item.AuthorType, item.CreatedAt);
                            break;
                    }

                    index++;

                    if (comment != null)
                    {
                        var errors = comment.Validate();
                        if (errors.Any()) { _errors.AddErrorRange(errors); }
                        else { Comments.Add(comment); }
                    }
                }

                CommentsCount = Comments.Count;
                // EF Hack
                CommentsConvert = Comments;
            }
            else { _errors.AddError(CommentsErrorMsg); }
        }

        public void AddBookmarkPins(List<BookmarkPin> bookmarkPins)
        {
            if (bookmarkPins != null && bookmarkPins.Count > 0)
            {
                BookmarkPins = new List<BookmarkPin>();

                foreach(var item in bookmarkPins)
                {
                    var pin = new BookmarkPin(
                        item.Id,
                        item.Index,
                        item.Range,
                        item.MediaUrl,
                        item.Comment);
                    var errors = pin.Validate();
                    if (errors.Any()) { _errors.AddErrorRange(errors); }
                    else { BookmarkPins.Add(pin); }
                }
            }
            // EF Hack
            BookmarkPinsConvert = BookmarkPins;
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
                    TicketId.CheckForNull(nameof(TicketId)),
                    TicketId.CheckLength(nameof(TicketId), 64),
                    EvaluatorId.CheckGreaterThanZero(nameof(EvaluatorId)),
                    AgentId.CheckGreaterThanZero(nameof(AgentId)),
                    EvaluatorName.CheckForNull(nameof(EvaluatorName)),
                    EvaluatorName.CheckLength(nameof(EvaluatorName), 128),
                    AgentName.CheckForNull(nameof(AgentName)),
                    AgentName.CheckLength(nameof(AgentName), 128)
                }.ClenupNullItems();
        }

        private string CreateReference()
        {
            string value = GetTicksExcludingFirst5Digits();
            return $"IC{value}";
        }
    }

    // For EF hacks
    public partial class InstantCoach
    {
        // EF stuff and hacks
        public string CommentsValue { get; private set; }
        public string BookmarkPinsValue { get; private set; }
        public List<Comment> CommentsConvert
        {
            get
            {
                if (string.IsNullOrWhiteSpace(BookmarkPinsValue))
                    return null;
                else
                {
                    var result = FromJson<List<Comment>>(CommentsValue);
                    Comments = result;
                    return result;
                }
            }
            private set { CommentsValue = ToJson(value); }
        }
        public List<BookmarkPin> BookmarkPinsConvert
        {
            get
            {
                if (string.IsNullOrWhiteSpace(BookmarkPinsValue))
                    return null;
                else
                {
                    var result = FromJson<List<BookmarkPin>>(BookmarkPinsValue);
                    BookmarkPins = result;
                    return result;
                }
            }
            private set
            {
                if (value == null)
                    BookmarkPinsValue = null;
                else
                    BookmarkPinsValue = ToJson(value);
            }
        }

        // Not really hack, but set via EF, and not handled by Domain.
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        // End EF stuff and hacks
    }
}