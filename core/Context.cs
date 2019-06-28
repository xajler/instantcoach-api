using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Entities;
using Core.Enums;

namespace Core.Context
{
    public class ICContext : DbContext
    {
        public ICContext(DbContextOptions<ICContext> options)
            : base(options)
        {
        }

        public DbSet<InstantCoach> InstantCoaches { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new InstantCoachConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            SetAuditInfo();
            return base.SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            SetAuditInfo();
            return await base.SaveChangesAsync();
        }

        private void SetAuditInfo()
        {
            var entries = ChangeTracker.Entries()
                                       .Where(x => x.Entity is Entity
                                                && (x.State == EntityState.Added
                                                    || x.State == EntityState.Modified));
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    ((Entity)entry.Entity).CreatedAt = DateTime.UtcNow;
                }

                ((Entity)entry.Entity).UpdatedAt = DateTime.UtcNow;
            }
        }
    }

    internal class InstantCoachConfiguration : IEntityTypeConfiguration<InstantCoach>
    {
        public void Configure(EntityTypeBuilder<InstantCoach> builder)
        {
            var statusConvert = new EnumToNumberConverter<InstantCoachStatus, byte>();
            builder.Property(x => x.Status)
                   .HasConversion(statusConvert);
            // String as JSON
            builder.Property(x => x.QuestionComments)
                   .HasColumnType("NVARCHAR(MAX)");
            // String as JSON
            builder.Property(x => x.BookmarkPins)
                   .HasColumnType("NVARCHAR(MAX)");
            builder.Property(x => x.Status)
                   .HasDefaultValue(InstantCoachStatus.New);
            builder.HasOne(x => x.Template)
                   .WithMany(x => x.InstantCoaches);
            builder.HasOne(x => x.Evaluator)
                   .WithMany(x => x.EvaluatorInstantCoaches)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Agent)
                   .WithMany(x => x.AgentInstantCoaches)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}