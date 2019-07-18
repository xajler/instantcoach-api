using System;
using System.ComponentModel.DataAnnotations;
using Domain;

namespace Core
{
    public class Config
    {
        public const string Name = "Config";

        [Required]
        public string DbHost { get; set; }
        [Required]
        public string DbName { get; set; }
        [Required]
        public string DbUser { get; set; }
        // TODO: Maybe make it encrypted in ENV_VAR and decrypt here
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
                throw new Exception($"Missing ENV Var: {envVar}");
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

        public string GetSUTConnectionString()
        {
            string host = GetEnvVarByName(DbHost);
            return $"Server={host};Initial Catalog=test_db_sut;User Id=sa;Password=Abc$12345;Integrated Security=false;MultipleActiveResultSets=True;";
        }

        public static string GetSUTGuidConnectionString()
        {
            string host = GetEnvVarByName("DB_HOST");
            return $"Server={host};Initial Catalog=test_db_sut_{Guid.NewGuid()};User Id=sa;Password=Abc$12345;Integrated Security=false;MultipleActiveResultSets=True;";
        }
    }
}