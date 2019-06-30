using System;
using System.Collections.Generic;
using Core.Enums;

using System.Data.Common;

namespace Core.Models
{
    public class ListResult<T> where T : class
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }
    }

    public class InstantCoachList
    {
        public int Id { get; set; }
        public InstantCoachStatus Status { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CommentsCount { get; set; }
        public int EvaluatorId { get; set; }
    }

    public class InstantCoach
    {
        public int Id { get; private set; }
        public string TicketId { get; private set; }
        public string Description { get; private set; }
        public int EvaluatorId { get; private set; }
        public List<Comment> Comments { get; private set; }
        public List<BookmarkPin> BookmarkPins { get; private set; }
    }

    public class InstantCoachDb
    {
        public InstantCoachDb(int id, string ticketId, string description,
            int evaluatorId, string comments, string bookmarkPins)
        {
            Id = id;
            TicketId = ticketId;
            Description = description;
            EvaluatorId = evaluatorId;
            Comments = comments;
            BookmarkPins = bookmarkPins;
        }

        public int Id { get; private set; }
        public string TicketId { get; private set; }
        public string Description { get; private set; }
        public int EvaluatorId { get; private set; }
        public string Comments { get; private set; }
        public string BookmarkPins { get; private set; }

        public static InstantCoachDb FromReader(DbDataReader reader)
        {
            string bookmarkPins = null;
            if (!reader.IsDBNull(5))
            {
                bookmarkPins = reader.GetString(5);
            }
            return new InstantCoachDb(
                id: reader.GetInt32(0),
                ticketId: reader.GetString(1),
                description: reader.GetString(2),
                evaluatorId: reader.GetInt32(3),
                comments: reader.GetString(4),
                bookmarkPins: bookmarkPins);
        }
    }

    public class InstantCoachCreate
    {
        public string Description { get; set; }
        public InstantCoachStatus Status => InstantCoachStatus.New;
        public string TicketId { get; set; }
        // TODO: Create reference from method
        public string Reference = "";
        public int EvaluatorId { get; set; }
        public int AgentId { get; set; }
        public int TemplateId { get; set; }
        public string Comments { get; set; }
        public string BookmarkPins { get; set; }
    }

    public class InstantCoachUpdate
    {
        public UpdateType UpdateType { get; set; }
        public string Comments { get; set; }
        public int CommentsCount { get; set; }
        public string BookmarkPins { get; set; }
    }

    public class InstantCoachCreateClient
    {
        public string Description { get; set; }
        public string TicketId { get; set; }
        public int EvaluatorId { get; set; }
        public int AgentId { get; set; }
        public int TemplateId { get; set; }
        public List<Comment> Comments { get; set; }
        public List<BookmarkPin> BookmarkPins { get; set; }
    }

    public class InstantCoachUpdateClient
    {
        public UpdateType UpdateType { get; set; }
        public List<Comment> Comments { get; set; }
        public List<BookmarkPin> BookmarkPins { get; set; }
    }

    public class BookmarkPin
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public int Range { get; set; }
        public string Comment { get; set; }
        public string MediaUrl { get; set; }
    }

    public class Comment
    {
        public CommentType CommentType { get; set; }
        public string Text { get; set; }
        public EvaluationCommentAuthor AuthorType { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PinId { get; set; }
    }
}
