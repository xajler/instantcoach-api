using System;
using System.Linq;

namespace Tests.Unit
{
    public static class TestHelpers
    {
        private static readonly Random random = new Random();

        public static string GenerateStringOfLength(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // public static T FromJson<T>(string json) where T : class
        // {
        //     return JsonConvert.DeserializeObject<T>(json);
        // }

        // public static string ToJson<T>(T value) where T : class
        // {
        //     return JsonConvert.SerializeObject(value);
        // }
    }
}