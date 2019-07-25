using System.Collections.Generic;

namespace Domain
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
        Textual,
        Bookmark,
        Attachment
    }

    public enum UpdateType
    {
        Save = 1,
        Review
    }

    public class ValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<string> Errors { get; private set; } = new List<string>();
        public void AddError(string error)
        {
            if (string.IsNullOrWhiteSpace(error)) { return; }
            Errors.Add(error);
        }
        public void AddErrorRange(IReadOnlyList<string> errors)
        {
            if (errors == null || errors.Count == 0) { return; }
            Errors.AddRange(errors);
        }
    }
}
