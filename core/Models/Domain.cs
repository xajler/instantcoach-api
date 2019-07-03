using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Core.Models
{
    public class InstantCoachList
    {
        public InstantCoachList(
            int id,
            InstantCoachStatus status,
            string reference,
            string description,
            DateTime createdAt,
            DateTime updatedAt,
            int commentsCount,
            string evaluatorName)
        {
            Id = id;
            Status = status;
            Reference = reference;
            Description = description;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            CommentsCount = commentsCount;
            EvaluatorName = evaluatorName;
        }
        public int Id { get; }
        public InstantCoachStatus Status { get; }
        public string Reference { get; }
        public string Description { get; }
        public DateTime CreatedAt { get; }
        public DateTime UpdatedAt { get; }
        public int CommentsCount { get; }
        public string EvaluatorName { get; }

        public static InstantCoachList FromReader(DbDataReader reader)
        {
            return new InstantCoachList(
                id: reader.GetInt32(1),
                status: (InstantCoachStatus)reader.GetByte(2),
                reference: reader.GetString(3),
                description: reader.GetString(4),
                createdAt: reader.GetDateTime(5),
                updatedAt: reader.GetDateTime(6),
                commentsCount: reader.GetInt32(7),
                evaluatorName: reader.GetString(8));
        }
    }

    public class InstantCoach
    {
        public InstantCoach(
            int id,
            string ticketId,
            string description,
            string evaluatorName,
            List<Comment> comments,
            List<BookmarkPin> bookmarkPins)
        {
            Id = id;
            TicketId = ticketId;
            Description = description;
            EvaluatorName = evaluatorName;
            Comments = comments;
            BookmarkPins = bookmarkPins;
        }

        public int Id { get; }
        public string TicketId { get; }
        public string Description { get; }
        public string EvaluatorName { get; }
        public List<Comment> Comments { get; }
        public List<BookmarkPin> BookmarkPins { get; }
    }

    public class InstantCoachCreate
    {
        public InstantCoachCreate(
            string description,
            InstantCoachStatus status,
            string ticketId,
            string reference,
            int evaluatorId,
            int agentId,
            string evaluatorName,
            string agentName,
            List<Comment> comments,
            List<BookmarkPin> bookmarkPins)
        {
            Description = description;
            Status = status;
            TicketId = ticketId;
            Reference = reference;
            EvaluatorId = evaluatorId;
            AgentId = agentId;
            EvaluatorName = evaluatorName;
            AgentName = agentName;
            Comments = comments;
            BookmarkPins = bookmarkPins;
        }

        public string Description { get; }
        public InstantCoachStatus Status { get; }
        public string TicketId { get; }
        public string Reference { get; }
        public int EvaluatorId { get; }
        public int AgentId { get; }
        public string EvaluatorName { get; }
        public string AgentName { get; }
        public List<Comment> Comments { get; }
        public List<BookmarkPin> BookmarkPins { get; }
    }

    public class InstantCoachUpdate
    {
        public InstantCoachUpdate(
            InstantCoachStatus status,
            List<Comment> comments,
            List<BookmarkPin> bookmarkPins)
        {
            Status = status;
            Comments = comments;
            BookmarkPins = bookmarkPins;
        }
        public InstantCoachStatus Status { get; }
        public List<Comment> Comments { get; }
        public List<BookmarkPin> BookmarkPins { get; }
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