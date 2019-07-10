using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tests.Intgration
{
    public static class TestHelpers
    {

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
}