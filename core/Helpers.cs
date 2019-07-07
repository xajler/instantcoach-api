using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Core.Models;

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

        public static void NotNull(object obj, string message)
        {
            if (obj == null)
            {
                throw new DomainAssertionException(message);
            }
        }

        public static void NotNullOrEmpty<T>(List<T> list, string message)
        {
            if (list == null || list.Count == 0)
            {
                throw new DomainAssertionException(message);
            }
        }

        public static void NotNullOrEmpty(string stringValue, string message)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                throw new DomainAssertionException(message);
            }
        }

        public static void NotEmptyOrContains(string stringValue, string containsText, string message)
        {
            if (string.IsNullOrWhiteSpace(stringValue)
                || !stringValue.Contains(containsText))
            {
                throw new DomainAssertionException(message);
            }
        }

        public static void GreaterThanZero(int value, string message)
        {
            if (value <= 0)
            {
                throw new DomainAssertionException(message);
            }
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