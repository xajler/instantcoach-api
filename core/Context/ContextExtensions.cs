using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System.IO;


namespace Core.Context
{
    public static class DbContextExtension
    {

        public static bool AllMigrationsApplied(this DbContext context)
        {
            var applied = context.GetService<IHistoryRepository>()
                .GetAppliedMigrations()
                .Select(m => m.MigrationId);

            var total = context.GetService<IMigrationsAssembly>()
                .Migrations
                .Select(m => m.Key);

            return !total.Except(applied).Any();
        }

        public static void EnsureSeeded(this ICContext context)
        {

            // if (!context.Type.Any())
            // {
            //     var types = JsonConvert.DeserializeObject<List<ThreatType>>(File.ReadAllText("seed" + Path.DirectorySeparatorChar + "types.json"));
            //     context.AddRange(types);
            //     context.SaveChanges();
            // }
        }
    }

}