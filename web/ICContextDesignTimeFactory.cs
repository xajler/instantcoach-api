using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Core.Context;

namespace InstantCoach
{
    //
    // NOTE:
    //
    // Neded only for running `dotnet ef` commands, not used for creating db context...
    //
    public class ICContextDesignTimeFactory : IDesignTimeDbContextFactory<ICContext>
    {
        public ICContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var connectionString = configuration.GetConnectionString("TestDb");
            var builder = new DbContextOptionsBuilder<ICContext>();
            builder.UseSqlServer(connectionString,
               providerOptions => providerOptions.CommandTimeout(180))
                                                 .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            return new ICContext(builder.Options);
        }
    }
}