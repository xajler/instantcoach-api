using System;
using Core.Enums;

namespace Core.Models
{
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
        public string Description { get; set; }
        public InstantCoachStatus Status { get; set; }
        public string TicketId { get; set; }
        public string Reference { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int EvaluatorId { get; set; }
        public int AgentId { get; set; }
        public int TemplateId { get; set; }
        public string QuestionComments { get; set; }
        public int QuestionCount { get; set; }
        public string BookmarkPins { get; set; }
    }
}
