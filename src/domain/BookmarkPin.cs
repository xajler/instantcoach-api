using System.Collections.Generic;
using Newtonsoft.Json;
using static Domain.Constants.Validation;

namespace Domain
{
    public sealed class Range : ValueObject
    {
        public Range(int start, int end)
        {
            Start = start;
            End = end;
        }

        public int Start { get; private set; }
        public int End { get; private set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Start;
            yield return End;
        }
    }

    public sealed class BookmarkPin : ValueObject
    {
        private readonly Dictionary<string, IReadOnlyCollection<string>> _errors
            = new Dictionary<string, IReadOnlyCollection<string>>();

        public BookmarkPin(int id, int index, Range range, string mediaurl)
            : this(id, index, range, mediaurl, null)
        {
        }

        [JsonConstructor]
        public BookmarkPin(int id, int index, Range range,
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

        public Dictionary<string, IReadOnlyCollection<string>> Validate(int atIndex)
        {
            if (Id <= 0)
            {
                _errors.Add(FullMemberName("Id", atIndex),
                    new List<string> { GreaterThanZeroMsg });
            }
            if (Index <= 0)
            {
                _errors.Add(FullMemberName("Index", atIndex),
                    new List<string> { GreaterThanZeroMsg }); }
            if (Range.Start <= 0)
            {
                _errors.Add(FullMemberName("Range.Start", atIndex),
                    new List<string> { GreaterThanZeroMsg });
            }
            if (Range.Start >= Range.End)
            {
                _errors.Add(FullMemberName("Range.End", atIndex),
                    new List<string> { "Should be greater than Range Start number." });
            }
            if (string.IsNullOrWhiteSpace(MediaUrl))
            {
                _errors.Add(FullMemberName("MediaUrl", atIndex),
                    new List<string> { RequiredMsg });
            }
            return _errors;
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
