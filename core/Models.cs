using System;

namespace Core.Models
{
    public enum InstantCoachStatus
    {
        New = 1,
        Waiting,
        Updated,
        InProgress,
        Completed
    }

    public class User
    {
        public int Id { get; set; }
    }

    public class Template
    {
        public int Id { get; set; }
    }

    public class InstantCoach
    {
        public int Id { get; set; }
        public string Description { get; set; }        // varchar 1000
        public InstantCoachStatus Status { get; set; } // tinyint(3)
        public string TicketId { get; set; }           // varchar 64
        public string Reference { get; set; }          // varchar 16
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int EvaluatorId { get; set; }
        public int AgentId { get; set; }
        public int TemplateId { get; set; }
        public string QuestionComments { get; set; }   // json
        public string QuestionCount { get; set; }
        public string BookmarkPins { get; set; }       // json
    }
}
