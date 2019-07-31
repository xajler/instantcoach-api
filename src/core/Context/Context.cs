using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain;

namespace Core.Context
{
    public sealed class ICContext : DbContext
    {
        public ICContext(DbContextOptions<ICContext> options)
            : base(options)
        {
        }

        public DbSet<InstantCoach> InstantCoaches { get; set; }

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

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            SetAuditInfo();
            return await SaveChangesAsync(true, cancellationToken);
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
                    ((IAuditable)entry.Entity).CreatedAt = DateTime.UtcNow;
                }

                ((IAuditable)entry.Entity).UpdatedAt = DateTime.UtcNow;
            }
        }
    }

    internal class InstantCoachConfiguration : IEntityTypeConfiguration<InstantCoach>
    {
        public void Configure(EntityTypeBuilder<InstantCoach> builder)
        {
            var statusConvert = new EnumToNumberConverter<InstantCoachStatus, byte>();
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   //.HasColumnName("Id")
                   .ForSqlServerUseSequenceHiLo("ic_hilo");
            builder.Property(x => x.Description)
                   .HasMaxLength(1000)
                   .IsRequired();
            builder.Property(x => x.Status)
                   .HasConversion(statusConvert)
                   .IsRequired();
            builder.Property(x => x.TicketId)
                   .HasMaxLength(64)
                   .IsRequired();
            builder.Property(x => x.Reference)
                   .HasMaxLength(16)
                   .IsRequired();
            builder.Property(x => x.EvaluatorId)
                   .IsRequired();
            builder.Property(x => x.AgentId)
                   .IsRequired();
            builder.Property(x => x.EvaluatorName)
                   .HasMaxLength(128)
                   .IsRequired();
            builder.Property(x => x.AgentName)
                   .HasMaxLength(128)
                   .IsRequired();
            // String as JSON
            builder.Property(x => x.CommentsValue)
                   .HasColumnName("Comments")
                   .HasColumnType("NVARCHAR(MAX)")
                   .IsRequired();
            // String as JSON
            builder.Property(x => x.BookmarkPinsValue)
                   .HasColumnName("BookmarkPins")
                   .HasColumnType("NVARCHAR(MAX)")
                   .IsRequired(false);

            builder.Ignore(x => x.Comments);
            // HACK: Converter from JSON to Domain List<Comment>
            builder.Ignore(x => x.CommentsConvert);
            builder.Ignore(x => x.BookmarkPins);
            // HACK: Converter from JSON to Domain List<BookmarkPin>
            builder.Ignore(x => x.BookmarkPinsConvert);
        }
    }
}