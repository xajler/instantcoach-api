namespace Core
{
    public static class Constants
    {
        public static string SUTEnv = "SUT";
        public static string VersionHeader = "X-Api-Version";
        public const string ResonseTimeHeader = "X-Response-Time";
        public const string PossibleBugText = "****POSSIBLE BUG******.";

        public static class Controller
        {
            public const string ApiRoute = "api/instantcoaches";
            public const string ApiVersion1 = "1.0";
            public const string ApiVersion2 = "2.0";
            public const string ProducesJsonContent = "application/json";
            public const string SchemaCreate = "create";
            public const string SchemaUpdate = "update";
            public const string SchemaDomain = "domain";
        }

        public static class Db
        {
            public const string GetAllStoreProcedure = "InstantCoach_List";
            public const string GetByIdQuery = @"SELECT Id, TicketId, Description, EvaluatorName, Comments, BookmarkPins
FROM InstantCoaches WHERE Id = @Id";
            public const string GetExistingIdQuery = "SELECT Id FROM InstantCoaches WHERE Id = @Id";
            public const string IdParam = "@Id";
            public const string SkipParam = "@Skip";
            public const string TakeParam = "@Take";
            public const string ShowCompletedParam = "@ShowCompleted";
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

            public const string DescriptionDesc = "";
            public const string TicketIdDesc = "";
            public const string EvaluatorIdDesc = "";
            public const string AgentIdDesc = "";
            public const string EvaluatorNameDesc = "";
            public const string AgentNameDesc = "";
            public const string CommentsDesc = "";
            public const string BookmarkPinsDesc = "";

            public const string UpdateTypeDesc = "";

            public const string CommentTypeDesc = @"";
            public const string TextDesc = "";
            public const string AuthorTypeDesc = "";
            public const string CreatedAtDesc = "";
            public const string BookmarkPinIdDesc = "";

            public const string BPIdDesc = "";
            public const string IndexDesc = "";
            public const string BPRangeDesc = "";
            public const string BPCommentDesc = "";
            public const string MediaUrlDesc = "";

            public const string RangeStartDesc = "Pin starting point in seconds. Required. Must be greater than 1.";
            public const string RangeEndDesc = "Pin ending point in seconds. Required. Must be greater than 1 and greater than starting point.";
        }
    }
}