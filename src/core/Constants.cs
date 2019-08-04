namespace Core
{
    public static class Constants
    {
        public const string SUTEnv = "SUT";
        public const string LocalEnv = "Local";
        public const string VersionHeader = "X-Api-Version";
        public const string ResponseTimeHeader = "X-Response-Time";
        public const string PossibleBugText = "****POSSIBLE BUG******.";
        public const string EsUrlEnVar = "ELASTICSEARCH_URL";

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
    }
}