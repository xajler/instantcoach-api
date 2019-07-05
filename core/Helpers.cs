using System;
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

        public static Result SuccessResult()
        {
            return new Result { Error = ErrorType.None };
        }

        public static Result<T> SuccessResult<T>(T value)
        {
            return new Result<T>
            {
                Value = value,
                Error = ErrorType.None
            };
        }

        public static Result<T> ErrorResult<T>(ErrorType errorType)
        {
            return new Result<T>{ Value = default, Error = errorType };
        }

        public static Result ErrorResult(ErrorType errorType)
        {
            return new Result { Error = errorType };
        }

        // TODO: Move to business logic

        public static string CreateReference()
        {
            string value = DateTime.UtcNow.Ticks.ToString().Substring(5);
            return $"IC{value}";
        }

        public static InstantCoachStatus SetStatus(UpdateType updateType)
        {
            if (updateType == UpdateType.Save)
            {
                return InstantCoachStatus.InProgress;
            }
            return InstantCoachStatus.Waiting;
        }
    }
}