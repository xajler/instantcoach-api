using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Domain;

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