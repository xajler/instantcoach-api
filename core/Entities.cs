using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Enums;

namespace Core.Entities
{
    public class Entity
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
    }

    public class User : Entity
    {
        public List<InstantCoach> EvaluatorInstantCoaches { get; set; }
        public List<InstantCoach> AgentInstantCoaches { get; set; }
    }

    public class Template : Entity
    {
        public List<InstantCoach> InstantCoaches { get; set; }
    }

    public class InstantCoach : Entity
    {
        [Required, MaxLength(1000)]
        public string Description { get; set; }
        [Required]
        public InstantCoachStatus Status { get; set; }
        [Required, MaxLength(64)]
        public string TicketId { get; set; }
        [Required, MaxLength(16)]
        public string Reference { get; set; }
        [Required]
        public int EvaluatorId { get; set; }
        public User Evaluator { get; set; }
        [Required]
        public int AgentId { get; set; }
        public User Agent { get; set; }
        [Required]
        public int TemplateId { get; set; }
        public Template Template { get; set; }
        [Required]
        public string QuestionComments { get; set; }
        [Required]
        public int QuestionCount { get; set; }
        public string BookmarkPins { get; set; }
    }
}