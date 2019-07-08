using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Domain
{
    public static class Helpers
    {
        public static string GetTicksExcludingFirst5Digits()
        {
            return DateTime.UtcNow.Ticks.ToString().Substring(4);
        }

        public static List<T> ClenupNullItems<T>(this List<T> items)
        {
            return items.Where(x => x != null).ToList();
        }

        public static T FromJson<T>(string json) where T : class
        {
            return JsonConvert.DeserializeObject<T>(json, GetJsonSettings());
        }

        public static string ToJson<T>(T value) where T : class
        {
            return JsonConvert.SerializeObject(value, GetJsonSettings());
        }

        private static JsonSerializerSettings GetJsonSettings()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };
            settings.Converters.Add(new StringEnumConverter());
            return settings;
        }
    }

    public static class IntExtensions
    {
        public static string CheckGreaterThanZero(this int value, string memberName)
        {
            if (value <= 0) return $"{memberName} should be greater than 0.";
            return null;
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
            var length = string.IsNullOrWhiteSpace(value) ? 0 : value.Length;
            //Console.WriteLine($"Check value of {memberName} is: {length} and max length is {maxLength}");
            if (length > maxLength)
                return $"{memberName} should not exceed {maxLength} characters.";
            return null;
        }

    }
}