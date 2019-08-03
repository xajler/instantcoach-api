using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using static Domain.Constants.Validation;

namespace Domain
{
    public static class Helpers
    {
        public static string GetTicksExcludingFirst5Digits()
        {
            return DateTime.UtcNow.Ticks.ToString().Substring(4);
        }

        public static List<T> CleanUpNullItems<T>(this List<T> items)
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
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };
            settings.Converters.Add(new StringEnumConverter());
            return settings;
        }
    }

    public static class IntExtensions
    {
        public static string CheckGreaterThanZero(this int value)
        {
            if (value <= 0) { return GreaterThanZeroMsg; }
            return null;
        }
    }

    public static class StringExtensions
    {
        public static string CheckForNull(this string value)
        {
            if (string.IsNullOrWhiteSpace(value)) { return RequiredMsg; }
            return null;
        }

        public static string CheckLength(this string value, int maxLength)
        {
            var length = string.IsNullOrWhiteSpace(value) ? 0 : value.Length;
            if (length > maxLength)
            {
                return $"Should not exceed {maxLength} characters.";
            }
            return null;
        }

    }
}