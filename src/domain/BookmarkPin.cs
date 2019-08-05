using System.Collections.Generic;
using Newtonsoft.Json;
using static Domain.Constants.Validation;

namespace Domain
{
    public sealed class Range : ValueObjectBase
    {
        [JsonConstructor]
        private Range(int start, int end)
        {
            Start = start;
            End = end;
        }

        public int Start { get; private set; }
        public int End { get; private set; }

        public static class Factory
        {
            public static Range Create(int start, int end)
            {
                return new Range(start, end);
            }
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Start;
            yield return End;
        }
    }

    public sealed class BookmarkPin : ValueObjectBase
    {
        [JsonConstructor]
        private BookmarkPin(int id, int index, Range range,
            string mediaurl, string comment)
        {
            Id = id;
            Index = index;
            Range = range;
            MediaUrl = mediaurl;
            Comment = comment;
        }

        public int Id { get; }
        public int Index { get; }
        public Range Range { get; }
        public string Comment { get; }
        public string MediaUrl { get; }

        public static class Factory
        {
            public static BookmarkPin Create(int id, int index, Range range,
                string mediaurl, string comment)
            {
                return new BookmarkPin(id, index, range, mediaurl, comment);
            }
        }

        public Dictionary<string, IReadOnlyCollection<string>> Validate(int atIndex)
        {
            var result = new Dictionary<string, IReadOnlyCollection<string>>();
            if (Id <= 0)
            {
                result.Add(FullMemberName("Id", atIndex),
                    new List<string> { GreaterThanZeroMsg });
            }
            if (Index <= 0)
            {
                result.Add(FullMemberName("Index", atIndex),
                    new List<string> { GreaterThanZeroMsg }); }
            if (Range.Start <= 0)
            {
                result.Add(FullMemberName("Range.Start", atIndex),
                    new List<string> { GreaterThanZeroMsg });
            }
            if (Range.Start >= Range.End)
            {
                result.Add(FullMemberName("Range.End", atIndex),
                    new List<string> { "Should be greater than Range Start number." });
            }
            if (string.IsNullOrWhiteSpace(MediaUrl))
            {
                result.Add(FullMemberName("MediaUrl", atIndex),
                    new List<string> { RequiredMsg });
            }
            return result;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Id;
            yield return Index;
            yield return Range;
            yield return MediaUrl;
            yield return Comment;
        }

        private static string FullMemberName(string memberName, int atIndex)
        {
            return $"BookmarkPins[{atIndex}].{memberName}";
        }
    }
}
