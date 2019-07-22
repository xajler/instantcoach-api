using System;
using System.Text;
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
    }

    public static class ExceptionExtensions
    {
        public static string ToInnerMessagesDump(this Exception exception)
        {
            if (exception == null) { return string.Empty; }

            var stringBuilder = new StringBuilder();
            int index = 0;

            while (exception != null)
            {
                if (index > 0)
                {
                    stringBuilder.AppendFormat("Inner Exception {0}", index).AppendLine();
                    stringBuilder.AppendLine("-------------------");
                    stringBuilder.AppendLine();
                }

                if (!string.IsNullOrEmpty(exception.Message))
                {
                    stringBuilder.AppendFormat("Message: {0}", exception.Message);
                }

                if (!string.IsNullOrEmpty(exception.StackTrace))
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendFormat("Stack Trace: {0}", exception.StackTrace);
                }

                if (index > 0)
                {
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("-------------------");
                    stringBuilder.AppendLine();
                }

                exception = exception.InnerException;
                index++;
            }

            return stringBuilder.ToString();
        }
    }
}