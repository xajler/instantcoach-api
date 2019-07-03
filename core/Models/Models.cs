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
        public ErrorType Error { get; set; }

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
        public T Value { get; set; }

        public override string ToString()
        {
            if (Success)
            {
                string result =  $"No errors. Error Type is: {Error}\n";
                if (EqualityComparer<T>.Default.Equals(Value, default))
                {
                    result += "Value has default value and probably shouldn't have";
                }
                return result;
            }

            return $"Error of type: {Error}";
        }
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
