using System;
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


    public static class DictionaryExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);
        }

        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dic, Dictionary<TKey, TValue> dicToAdd)
        {
            dicToAdd.ForEach(x => dic.Add(x.Key, x.Value));
        }
    }

    public sealed class ValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public Dictionary<string, IReadOnlyCollection<string>> Errors { get; private set; }
            = new Dictionary<string, IReadOnlyCollection<string>>();
        public void AddError(string member, IReadOnlyCollection<string> errors)
        {
            if (string.IsNullOrWhiteSpace(member)) { return; }
            if (errors == null || errors.Count == 0) { return; }
            Errors.Add(member, errors);
        }
    }
}
