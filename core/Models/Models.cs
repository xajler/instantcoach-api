using System;
using System.Collections.Generic;
using System.Data.Common;

namespace Core.Models
{
    public class ListResult<T> where T : class
    {
        public IReadOnlyCollection<T> Items { get; set; }
        public int TotalCount { get; set; }
    }

    public class Result
    {
        public bool Success => Error == ErrorType.None;
        protected ErrorType Error { get; set; }

        public static Result AsSuccess()
        {
            return new Result { Error = ErrorType.None };
        }

        public static Result AsError(ErrorType errorType)
        {
            return new Result { Error = errorType };
        }

        public override string ToString()
        {
            if (Success)
            {
                return $"No errors. Error Type is: {Error}";
            }
            return $"Error of type: {Error}";
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

        public override string ToString()
        {
            if (Success)
            {
                string result = $"No errors. Error Type is: {Error}\n";
                if (EqualityComparer<T>.Default.Equals(Value, default))
                {
                    result += "Value has default value and probably shouldn't have";
                }
                return result;
            }

            return $"Error of type: {Error}";
        }
    }

    public class ValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<string> Errors { get; private set; } = new List<string>();
        public void AddError(string error)
        {
            if (string.IsNullOrWhiteSpace(error)) return;
            Errors.Add(error);
        }
        public void AddErrorRange(List<string> errors)
        {
            if (errors == null || errors.Count == 0) return;
            Errors.AddRange(errors);
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

    public class InstantCoachCreateClient
    {
        public string Description { get; set; }
        public string TicketId { get; set; }
        public int EvaluatorId { get; set; }
        public int AgentId { get; set; }
        public string EvaluatorName { get; set; }
        public string AgentName { get; set; }
        public List<Comment> Comments { get; set; }
        public List<BookmarkPin> BookmarkPins { get; set; }
    }

    public class InstantCoachUpdateClient
    {
        public UpdateType UpdateType { get; set; }
        public List<Comment> Comments { get; set; }
        public List<BookmarkPin> BookmarkPins { get; set; }
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
