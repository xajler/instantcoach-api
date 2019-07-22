using System.Collections.Generic;

namespace Domain
{
    public class Range : ValueObject
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

    public class BookmarkPin : ValueObject
    {
        private readonly List<string> _errors = new List<string>();

        public BookmarkPin(int id, int index, Range range, string mediaurl)
            : this(id, index, range, mediaurl, null)
        {
        }

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

        public IReadOnlyList<string> Validate()
        {
            if (Id <= 0) { _errors.Add("Bookmark Pin Id should be greater than 0."); }
            if (Index <= 0) { _errors.Add("Bookmark Pin Index should be greater than 0."); }
            if (Range.Start <= 0) { _errors.Add("Bookmark Pin Range Start should be greater than 0."); }
            if (Range.Start >= Range.End)
            {
                _errors.Add("Bookmark Pin Range end number must be greater than start number.");
            }
            if (string.IsNullOrWhiteSpace(MediaUrl))
            {
                _errors.Add("Bookmark Pin MediaUrl is required.");
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
    }
}