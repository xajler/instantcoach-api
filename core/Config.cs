using System;
using System.ComponentModel.DataAnnotations;
using Core.Models;

namespace Core
{
    public class Config
    {
        public const string Name = "Config";
        public const string ApiRoute = "api/instantcoaches";
        public const string ApiVersion1 = "1";
        public const string ApiVersion2 = "2";
        public const string ProducesJsonContent = "application/json";

        [Required]
        public string DbName { get; set; }
        [Required]
        public string DbUser { get; set; }
        // TODO: Maybe make it encrypted in ENV_VAR and decrypt here
        [Required]
        public string DbPassword { get; set; }
        [Required]
        public InstantCoachStatus InstantCoachStatusDefault { get; set; }

        public string GetEnvVarByName(string envVar)
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
            string name = GetEnvVarByName(DbName);
            string user = GetEnvVarByName(DbUser);
            string password = GetEnvVarByName(DbPassword);
            return $"Data Source=localhost;Initial Catalog={name};User Id={user};Password={password};Integrated Security=false;MultipleActiveResultSets=True;";
        }
    }
}