using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Core;
using Core.Context;
using static System.Console;

namespace Api
{
    //
    // NOTE:
    //
    // Needed only for running `dotnet ef` commands, not used for creating db context...
    //
    public class ICContextDesignTimeFactory : IDesignTimeDbContextFactory<ICContext>
    {
        public ICContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            Config config = configuration.GetSection(Config.Name).Get<Config>();
            var connectionString = config.GetConnectionString();
            var builder = new DbContextOptionsBuilder<ICContext>();
            builder.UseSqlServer(connectionString);
            return new ICContext(builder.Options);
        }
    }
}