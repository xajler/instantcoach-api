using System.Collections.Generic;
using Core.Domain;
using static Core.Helpers;

namespace Core.Models
{
    public static class ModelsExtensions
    {
        public static InstantCoach ToInstantCoach(
            this InstantCoachCreateClient create)
        {
            return new InstantCoach(
                description: create.Description,
                ticketId: create.TicketId,
                evaluatorId: create.EvaluatorId,
                agentId: create.AgentId,
                evaluatorName: create.EvaluatorName,
                agentName: create.AgentName,
                comments: CreateCommentsFromClient(create.Comments),
               // commentsCount: create.Comments.Count,
                bookmarkPins: CreateBookmarkPinsFromClient(create.BookmarkPins)

            );
        }

        // TODO: move to factory or something
        private static List<Comment> CreateCommentsFromClient(List<CommentClient> data)
        {
            var result = new List<Comment>();
            if (data == null || data.Count == 0) { return result; }

            foreach(var item in data)
            {
                switch (item.CommentType)
                {
                    case CommentType.Textual:
                        result.Add(Comment.Textual(item.Text, item.AuthorType, item.CreatedAt));
                        break;
                    case CommentType.Attachment:
                        result.Add(Comment.Attachment(item.Text, item.AuthorType, item.CreatedAt));
                        break;
                    case CommentType.Bookmark:
                        result.Add(Comment.Bookmark(item.BookmarkPinId, item.AuthorType, item.CreatedAt));
                        break;
                    default:
                        // TODO do logging
                        break;
                }
            }

            return result;
        }

        // TODO: move to factory or something
        private static List<BookmarkPin> CreateBookmarkPinsFromClient(List<BookmarkPinClient> data)
        {
            var result = new List<BookmarkPin>();
            if (data == null || data.Count == 0) { return result; }

            foreach (var item in data)
            {
                result.Add(new BookmarkPin
                {
                    Id = item.Id,
                    Index = item.Index,
                    Range = item.Range,
                    Comment = item.Comment,
                    MediaUrl = item.MediaUrl
                });
            }

            return result;
        }

        // public static InstantCoachDbEntity ToInstantCoachDbEntity(
        //     this InstantCoachUpdate update,
        //     InstantCoachDbEntity currentState,
        //     (int, string) commentsWithCount)
        // {
        //     (int count, string comments) = commentsWithCount;
        //     var result = currentState;
        //     result.Status = update.Status;
        //     result.Comments = comments;
        //     result.CommentsCount = count;
        //     result.BookmarkPins = ToJson(update.BookmarkPins);
        //     return result;
        // }

        public static InstantCoachForId ToInstantCoachForId(this InstantCoachDb db)
        {
            return new InstantCoachForId(
                id: db.Id,
                ticketId: db.TicketId,
                description: db.Description,
                evaluatorName: db.EvaluatorName,
                comments: FromJson<List<Comment>>(db.Comments),
                bookmarkPins: FromJson<List<BookmarkPin>>(db.BookmarkPins));
        }

        // public static InstantCoachUpdate ToInstantCoachUpate(
        //     this InstantCoachUpdateClient clientData,
        //     InstantCoachStatus status)
        // {
        //     return new InstantCoachUpdate(
        //         status: status,
        //         comments: clientData.Comments,
        //         bookmarkPins: clientData.BookmarkPins
        //     );
        // }
    }
}