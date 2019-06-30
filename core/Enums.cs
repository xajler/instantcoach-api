namespace Core.Enums
{
    public enum InstantCoachStatus
    {
        New = 1,
        Waiting,
        Updated,
        InProgress,
        Completed
    }

    public enum EvaluationCommentAuthor
    {
        Evaluator = 1,
        Agent
    }

    public enum CommentType
    {
        Text = 1,
        Bookmark,
        Attachment
    }

    public enum UpdateType
    {
        Save = 1,
        Review
    }
}