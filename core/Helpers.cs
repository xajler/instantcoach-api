using Newtonsoft.Json;

namespace Core
{
    public static class Helpers
    {
        public static T FromJson<T>(string json) where T : class
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}