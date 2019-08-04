namespace Domain
{
    public static class Constants
    {
        public static class Validation
        {
            public const string RequiredMsg = "Requires a value.";
            public const string GreaterThanZeroMsg = "Should be greater than 0.";
        }
        public static class Model
        {
            public const string CreateDisplayName = "Create InstantCoach";
            public const string CreateDesc = "The client model for creating InstantCoach.";
            public const string UpdateDisplayName = "Update InstantCoach";
            public const string UpdateDesc = "The client model for update InstantCoach.";
            public const string CommentDisplayName = "InstantCoach Comments";
            public const string CommentDesc = "The client model for InstantCoach comments.";
            public const string BookmarkPinDisplayName = "InstantCoach Bookmark Pins";
            public const string BookmarkPinDesc = "The client model for InstantCoach bookmark pins.";
            public const string RangeDisplayName = "Bookmark Pin Range";
            public const string RangeDesc = "The client model for bookmark pin ranging start and end of pin in seconds.";

            public const string DescriptionDesc = "The Description of InstantCoach. Required. Max length is 1000 characters.";
            public const string TicketIdDesc = "The Id of Ticket for InstantCoach. Required. Should be representation of id int, long, GUID. Max length is 64 characters.";
            public const string EvaluatorIdDesc = "The Evaluator Id of InstantCoach. Required. Should be valid integer and greater than 1.";
            public const string AgentIdDesc = "The Agent Id of InstantCoach. Required. Should be valid integer and greater than 1.";
            public const string EvaluatorNameDesc = "The Evaluator name at time of creating InstantCoach. Required. Max length is 128 characters.";
            public const string AgentNameDesc = "The Agent name at time of creating InstantCoach. Required. Max length is 128 characters.";
            public const string CommentsDesc = "The InstantCoach Comments. Required. Must be at least one comment.";
            public const string BookmarkPinsDesc = "The Bookmark Pins used in Comments. Only if comment of type bookmark.";

            public const string UpdateTypeDesc = "The type of InstantCoach Update, can be of 'Save' or 'Review'. When 'Save' then InstantCoach Status is set to 'In Progress', on 'Review' type is set to be 'Waiting'.";

            public const string CommentTypeDesc = "The type of Comment, can be 'Textual', 'Attachment' and 'Bookmark'. Required.";
            public const string TextDesc = "The Comment text, required for 'Textual' and 'Attachment' type. Max length is 1000 characters.";
            public const string AuthorTypeDesc = "The comment author, can be either 'Evaluator' or 'Agent'. Required.";
            public const string CreatedAtDesc = "The date and time when Comment is created. Used for chronological view of comments. Required.";
            public const string BookmarkPinIdDesc = "The bookmark pin id for Comment. Null by default, but required when comment type is 'Bookmark'.";

            public const string BPIdDesc = "The Id for Bookmark Pin. Used as reference in Comment of type 'Bookmark'. Required. Should be valid integer and greater than 1.";
            public const string IndexDesc = "The index of bookmark pin, used to show pins in order. Required. Should be valid integer and greater than 1.";
            public const string BPRangeDesc = "The range of pin with starting and ending point in seconds. Required.";
            public const string BPCommentDesc = "The comment about Bookmark Pin. Max length is 1000 characters.";
            public const string MediaUrlDesc = "The Bookmark pin media url.";

            public const string RangeStartDesc = "Pin starting point in seconds. Required. Must be greater than 1.";
            public const string RangeEndDesc = "Pin ending point in seconds. Required. Must be greater than 1 and greater than starting point.";
        }
    }
}