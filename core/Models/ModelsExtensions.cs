using System;
using System.Collections.Generic;
using Domain;
using static Core.Helpers;

namespace Core.Models
{
    public static class ModelsExtensions
    {
        public static InstantCoach ToNewInstantCoach(
            this InstantCoachCreateClient create)
        {
            var result = new InstantCoach(
                description: create.Description,
                ticketId: create.TicketId,
                evaluatorId: create.EvaluatorId,
                agentId: create.AgentId,
                evaluatorName: create.EvaluatorName,
                agentName: create.AgentName
            );
            result.AddComments(create.Comments.ToComments());
            result.AddBookmarkPins(create.BookmarkPins.ToBookmarkPins());
            return result;
        }

        public static List<Comment> ToComments(this List<CommentClient> comments)
        {
            List<Comment> result = null;

            if (comments != null && comments.Count > 0)
            {
                result = new List<Comment>();
                foreach (var item in comments)
                {
                    result.Add(CreateComment(item));
                }
            }

            return result;
        }

        public static List<BookmarkPin> ToBookmarkPins(this List<BookmarkPinClient> bookmarkPins)
        {
            List<BookmarkPin> result = null;

            if (bookmarkPins != null && bookmarkPins.Count > 0)
            {
                result = new List<BookmarkPin>();

                foreach (var item in bookmarkPins)
                {
                    var pin = new BookmarkPin(
                        item.Id,
                        item.Index,
                        new Range(item.Range.Start, item.Range.End),
                        item.MediaUrl,
                        item.Comment);

                    result.Add(pin);
                }
            }
            return result;
        }

        public static InstantCoachForId ToInstantCoachForId(this InstantCoachDb db)
        {
            List<BookmarkPin> bookmarkPins = null;
            if (!string.IsNullOrWhiteSpace(db.BookmarkPins))
            {
                bookmarkPins = FromJson<List<BookmarkPin>>(db.BookmarkPins);
            }
            return new InstantCoachForId(
                id: db.Id,
                ticketId: db.TicketId,
                description: db.Description,
                evaluatorName: db.EvaluatorName,
                comments: FromJson<List<Comment>>(db.Comments),
                bookmarkPins);
        }

        private static Comment CreateComment(CommentClient item)
        {
            switch (item.CommentType)
            {
                case CommentType.Textual:
                    return Comment.Textual(item.Text, item.AuthorType, item.CreatedAt);
                case CommentType.Attachment:
                    return Comment.Attachment(item.Text, item.AuthorType, item.CreatedAt);
                case CommentType.Bookmark:
                    return Comment.Bookmark(item.BookmarkPinId, item.AuthorType, item.CreatedAt);
                default:
                    throw new ArgumentOutOfRangeException($"Unknown comment: {item.CommentType}");
            }
        }
    }
}