using System;
using System.Collections.Generic;
using System.Linq;
using Domain;

namespace Tests.Unit
{
    public sealed class TestEntity : Entity
    {
        public TestEntity(int id)
        {
            base.UpdateId(id);
        }
    }

    public static class TestHelpers
    {
        private static readonly Random random = new Random();

        public static string GenerateStringOfLength(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public static (string, IReadOnlyCollection<string>) CreateExpectedValues(
            string prefix, string memberName, int atIndex, string errorText)
        {
            return ($"{prefix}[{atIndex}].{memberName}", new List<string> { errorText });
        }
    }
}