using System;
using System.ComponentModel.DataAnnotations;
using Domain;


namespace Core
{
    public sealed class Config
    {
        public const string Name = "Config";

        [Required]
        public string DbHost { get; set; }
        [Required]
        public string DbName { get; set; }
        [Required]
        public string DbUser { get; set; }
        [Required]
        public string DbPassword { get; set; }
        [Required]
        public InstantCoachStatus InstantCoachStatusDefault { get; set; }
        [Required]
        public string JwtAuthority { get; set; }
        [Required]
        public string JwtAudience { get; set; }


        public static string GetEnvVarByName(string envVar)
        {
            string result = Environment.GetEnvironmentVariable(envVar);
            if (string.IsNullOrEmpty(result))
            {
                throw new ArgumentNullException($"Missing ENV Var: {envVar}");
            }
            return result;
        }

        public string GetConnectionString()
        {
            string host = GetEnvVarByName(DbHost);
            string name = GetEnvVarByName(DbName);
            string user = GetEnvVarByName(DbUser);
            string password = GetEnvVarByName(DbPassword);
            return $"Server={host};Initial Catalog={name};User Id={user};Password={password};Integrated Security=false;MultipleActiveResultSets=True;";
        }

        public string GetSutConnectionString()
        {
            string host = GetEnvVarByName(DbHost);
            return $"Server={host};Initial Catalog=test_db_sut;User Id=sa;Password=Abc$12345;Integrated Security=false;MultipleActiveResultSets=True;";
        }

        public static string GetSutGuidConnectionString()
        {
            string host = GetEnvVarByName("DB_HOST");
            return $"Server={host};Initial Catalog=test_db_sut_{Guid.NewGuid()};User Id=sa;Password=Abc$12345;Integrated Security=false;MultipleActiveResultSets=True;";
        }
    }
}