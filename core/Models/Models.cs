using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Common;
using Domain;
using static Core.Constants.Model;

namespace Core.Models
{
    public enum ErrorType
    {
        None,
        UnknownId,
        InvalidData,
        SaveChangesFailed
    }

    public class ListResult<T> where T : class
    {
        public IReadOnlyCollection<T> Items { get; set; }
        public int TotalCount { get; set; }
    }

    public class Result
    {
        public bool Success => Error == ErrorType.None;
        public ErrorType Error { get; set; }
        public IReadOnlyList<string> Errors { get; protected set; }

        public static Result AsSuccess()
        {
            return new Result { Error = ErrorType.None };
        }

        public static Result AsError(ErrorType errorType)
        {
            return new Result { Error = errorType };
        }

        public static Result AsDomainError(IReadOnlyList<string> errors)
        {
            return new Result
            {
                Error = ErrorType.InvalidData,
                Errors = errors
            };
        }
    }

    public class Result<T> : Result
    {
        public T Value { get; private set; }

        public static Result<T> AsSuccess(T value)
        {
            return new Result<T>
            {
                Value = value,
                Error = ErrorType.None
            };
        }

        public static new Result<T> AsError(ErrorType errorType)
        {
            return new Result<T> { Value = default, Error = errorType };
        }

        public static new Result<T> AsDomainError(IReadOnlyList<string> errors)
        {
            return new Result<T>
            {
                Error = ErrorType.InvalidData,
                Errors = errors
            };
        }
    }

    public class CreatedId
    {
        public CreatedId(int id) => Id = id;
        public int Id { get; }
    }

    // Return Models
    // Read-Only For List and GetById
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

    public class InstantCoachForId
    {
        public InstantCoachForId(
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

    // Client (Body) Models

    [DisplayName(CreateDisplayName)]
    [Description(CreateDesc)]
    public class InstantCoachCreateClient
    {
        [Required, MaxLength(1000)]
        [Description(DescriptionDesc)]
        public string Description { get; set; }
        [Required, MaxLength(64)]
        [DisplayName("Ticket Id")]
        [Description(TicketIdDesc)]
        public string TicketId { get; set; }
        [Required, Range(1, int.MaxValue)]
        [DisplayName("Evaluator Id")]
        [Description(EvaluatorIdDesc)]
        public int EvaluatorId { get; set; }
        [Required, Range(1, int.MaxValue)]
        [DisplayName("Agent Id")]
        [Description(AgentIdDesc)]
        public int AgentId { get; set; }
        [Required, MaxLength(128)]
        [DisplayName("Evaluator Name")]
        [Description(EvaluatorNameDesc)]
        public string EvaluatorName { get; set; }
        [Required, MaxLength(128)]
        [DisplayName("Agent Name")]
        [Description(AgentNameDesc)]
        public string AgentName { get; set; }
        [Required]
        [Description(CommentsDesc)]
        public List<CommentClient> Comments { get; set; }
        [DisplayName("Bookmark Pins")]
        [Description(BookmarkPinsDesc)]
        public List<BookmarkPinClient> BookmarkPins { get; set; }
    }

    [DisplayName(UpdateDisplayName)]
    [Description(UpdateDesc)]
    public class InstantCoachUpdateClient
    {
        [Required]
        [DisplayName("Update Type")]
        [Description(UpdateTypeDesc)]
        public UpdateType UpdateType { get; set; }
        [Required]
        [Description(CommentsDesc)]
        public List<CommentClient> Comments { get; set; }
        [DisplayName("Bookmark Pins")]
        [Description(BookmarkPinsDesc)]
        public List<BookmarkPinClient> BookmarkPins { get; set; }
    }

    [DisplayName(CommentDisplayName)]
    [Description(CommentDesc)]
    public class CommentClient
    {
        [Required]
        [DisplayName("Comment Type")]
        [Description(CommentTypeDesc)]
        public CommentType CommentType { get; set; }
        [MaxLength(1000)]
        [Description(TextDesc)]
        public string Text { get; set; }
        [Required]
        [DisplayName("Author Type")]
        [Description(AuthorTypeDesc)]
        public EvaluationCommentAuthor AuthorType { get; set; }
        [Required]
        [DisplayName("Created At")]
        [Description(CreatedAtDesc)]
        public DateTime CreatedAt { get; set; }
        [DisplayName("Bookmark Pin Id")]
        [Description(BookmarkPinIdDesc)]
        public int? BookmarkPinId { get; set; }
    }

    [DisplayName(BookmarkPinDisplayName)]
    [Description(BookmarkPinDesc)]
    public class BookmarkPinClient
    {
        [Required, Range(1, int.MaxValue)]
        [Description(BPIdDesc)]
        public int Id { get; set; }
        [Required, Range(1, int.MaxValue)]
        [Description(IndexDesc)]
        public int Index { get; set; }
        [Description(BPRangeDesc)]
        public RangeClient Range { get; set; }
        [Required, MaxLength(1000)]
        [Description(BPCommentDesc)]
        public string Comment { get; set; }
        [Required, MaxLength(1000)]
        [Description(MediaUrlDesc)]
        public string MediaUrl { get; set; }
    }

    [DisplayName(RangeDisplayName)]
    [Description(RangeDesc)]
    public class RangeClient
    {
        [Required, Range(1, int.MaxValue)]
        [Description(RangeStartDesc)]
        public int Start { get; set; }
        [Required, Range(1, int.MaxValue)]
        [Description(RangeEndDesc)]
        public int End { get; set; }
    }

    // Db Models

    public class InstantCoachDb
    {
        public InstantCoachDb(
            int id,
            string ticketId,
            string description,
            string evaluatorName,
            string comments,
            string bookmarkPins)
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
        public string Comments { get; }
        public string BookmarkPins { get; }

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
                evaluatorName: reader.GetString(3),
                comments: reader.GetString(4),
                bookmarkPins: bookmarkPins);
        }
    }
}
