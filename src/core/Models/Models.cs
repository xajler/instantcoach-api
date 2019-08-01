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

    public sealed class ListResult<T> where T : class
    {
        public IReadOnlyCollection<T> Items { get; set; }
        public int TotalCount { get; set; }
    }

    public class Result
    {
        public bool Success => Error == ErrorType.None;
        public ErrorType Error { get; set; }
        public Dictionary<string, IReadOnlyCollection<string>> Errors { get; protected set; }

        public static Result AsSuccess()
        {
            return new Result { Error = ErrorType.None };
        }

        public static Result AsError(ErrorType errorType)
        {
            return new Result { Error = errorType };
        }

        public static Result AsDomainError(
            Dictionary<string, IReadOnlyCollection<string>> errors)
        {
            return new Result
            {
                Error = ErrorType.InvalidData,
                Errors = errors
            };
        }
    }

    public sealed class Result<T> : Result
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

        public static new Result<T> AsDomainError(
            Dictionary<string, IReadOnlyCollection<string>> errors)
        {
            return new Result<T>
            {
                Error = ErrorType.InvalidData,
                Errors = errors
            };
        }
    }

    public sealed class CreatedId
    {
        public CreatedId(int id) => Id = id;
        public int Id { get; }
    }

    public sealed class SqlParamValue
    {
        public string Param { get; set; }
        public string Value { get; set; }

        public static SqlParamValue ToSelf(string paramName, string value)
        {
            return new SqlParamValue { Param = paramName, Value = value };
        }
    }

    // Return Models
    // Read-Only For List and GetById
    public sealed class InstantCoachList
    {
        public int Id { get; set; }
        public InstantCoachStatus Status { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CommentsCount { get; set; }
        public string EvaluatorName { get; set; }

        public static InstantCoachList FromReader(DbDataReader reader)
        {
            return new InstantCoachList
            {
                Id = reader.GetInt32(1),
                Status = (InstantCoachStatus)reader.GetByte(2),
                Reference = reader.GetString(3),
                Description = reader.GetString(4),
                CreatedAt = reader.GetDateTime(5),
                UpdatedAt = reader.GetDateTime(6),
                CommentsCount = reader.GetInt32(7),
                EvaluatorName = reader.GetString(8)
            };
        }
    }

    public sealed class InstantCoachForId
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
    public sealed class InstantCoachCreateClient
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
    public sealed class InstantCoachUpdateClient
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
    public sealed class CommentClient
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
    public sealed class BookmarkPinClient
    {
        [Required, Range(1, int.MaxValue)]
        [Description(BPIdDesc)]
        public int Id { get; set; }
        [Required, Range(1, int.MaxValue)]
        [Description(IndexDesc)]
        public int Index { get; set; }
        [Description(BPRangeDesc)]
        public RangeClient Range { get; set; }
        [MaxLength(1000)]
        [Description(BPCommentDesc)]
        public string Comment { get; set; }
        [Required, MaxLength(1000)]
        [Description(MediaUrlDesc)]
        public string MediaUrl { get; set; }
    }

    [DisplayName(RangeDisplayName)]
    [Description(RangeDesc)]
    public sealed class RangeClient
    {
        [Required, Range(1, int.MaxValue)]
        [Description(RangeStartDesc)]
        public int Start { get; set; }
        [Required, Range(1, int.MaxValue)]
        [Description(RangeEndDesc)]
        public int End { get; set; }
    }

    // Db Models

    public sealed class InstantCoachDb
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