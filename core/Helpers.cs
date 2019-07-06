using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Core
{
    public static class Helpers
    {
        public static T FromJson<T>(string json) where T : class
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string ToLogJson<T>(T value)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
            settings.Converters.Add(new StringEnumConverter());
            return JsonConvert.SerializeObject(value, settings);
        }

        public static string ToJson<T>(T value) where T : class
        {
            return JsonConvert.SerializeObject(value);
        }

        public static string CheckStringForLength(string value, string memberName, int length)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return $"{memberName} should not exceed {length} charactes";
            }
            return "";
        }

        public static List<T> ClenupNullItems<T>(this List<T> items)
        {
            return items.Where(x => x != null).ToList();
        }
    }

    public static class StringExtensions
    {
        public static string CheckForNull(this string value, string memberName)
        {
            if (string.IsNullOrWhiteSpace(value)) return $"{memberName} is required.";
            return null;
        }

        public static string CheckLength(this string value, string memberName, int maxLength)
        {
            if (value.Length > maxLength)
                return $"{memberName} should not exceed {maxLength} charactes.";
            return null;
        }

        public static string CheckExactLength(this string value, string memberName, int maxLength)
        {
            if (value.Length != maxLength)
                return $"{memberName} should be always of {maxLength} charactes.";
            return null;
        }
    }
}