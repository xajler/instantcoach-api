using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static Domain.Constants.Model;

namespace Domain
{
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
}