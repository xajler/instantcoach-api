using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Core.Context;

namespace Core
{
    //
    // NOTE:
    //
    // Neded only for running `dotnet ef` commands, not used for creating db context...
    //
    public class ICContextDesignTimeFactory : IDesignTimeDbContextFactory<ICContext>
    {
        // TODO: change from hardcode but it can be hardcoded, needed only of ef commands.
        private const string DbConnString = "Data Source=localhost;Initial Catalog=test;User Id=sa;Password=Abc$12345;Integrated Security=false;MultipleActiveResultSets=True;";

        public ICContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ICContext>();
            builder.UseSqlServer(DbConnString,
               providerOptions => providerOptions.CommandTimeout(180))
                                                 .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            return new ICContext(builder.Options);
        }
    }
}