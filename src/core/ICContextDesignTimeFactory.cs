using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Core.Context;
using static System.Console;

namespace Core
{
    //
    // NOTE:
    //
    // Needed only for running `dotnet ef` commands, not used for creating db context...
    //
    public sealed class ICContextDesignTimeFactory : IDesignTimeDbContextFactory<ICContext>
    {
        public ICContext CreateDbContext(string[] args)
        {
            var connectionString = $"Data Source=localhost;Initial Catalog=test-local;User Id=sa;Password=Abc$12345;Integrated Security=false;MultipleActiveResultSets=True;";
            WriteLine($"conn string: {connectionString}");
            var builder = new DbContextOptionsBuilder<ICContext>();
            builder.UseSqlServer(connectionString);
            return new ICContext(builder.Options);
        }
    }
}