using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Core.Models;

namespace Core.Context
{
    public class ICContext : DbContext
    {
        public ICContext(DbContextOptions<ICContext> options)
            : base(options)
        {
        }

        public DbSet<InstantCoachDbEntity> InstantCoaches { get; set; }

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
            return (await base.SaveChangesAsync(true, cancellationToken));
        }

        private void SetAuditInfo()
        {
            var entries = ChangeTracker.Entries()
                                       .Where(x => x.Entity is DbEntity
                                                && (x.State == EntityState.Added
                                                    || x.State == EntityState.Modified));
            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    ((DbEntity)entry.Entity).CreatedAt = DateTime.UtcNow;
                }

                ((DbEntity)entry.Entity).UpdatedAt = DateTime.UtcNow;
            }
        }
    }

    public class DbEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
    }

    public class InstantCoachDbEntity : DbEntity
    {
        [Required, MaxLength(1000)]
        public string Description { get; set; }
        [Required]
        public InstantCoachStatus Status { get; set; }
        [Required, MaxLength(64)]
        public string TicketId { get; set; }
        [Required, MaxLength(16)]
        public string Reference { get; set; }
        [Required]
        public int EvaluatorId { get; set; }
        [Required]
        public int AgentId { get; set; }
        [Required, MaxLength(128)]
        public string EvaluatorName { get; set; }
        [Required, MaxLength(128)]
        public string AgentName { get; set; }
        [Required]
        public string Comments { get; set; }
        [Required]
        public int CommentsCount { get; set; }
        public string BookmarkPins { get; set; }
    }

    internal class InstantCoachConfiguration : IEntityTypeConfiguration<InstantCoachDbEntity>
    {
        public void Configure(EntityTypeBuilder<InstantCoachDbEntity> builder)
        {
            var statusConvert = new EnumToNumberConverter<InstantCoachStatus, byte>();
            builder.Property(x => x.Id)
                   .ForSqlServerUseSequenceHiLo("ic_hilo");
            builder.Property(x => x.Status)
                   .HasConversion(statusConvert);
            // String as JSON
            builder.Property(x => x.Comments)
                   .HasColumnType("NVARCHAR(MAX)");
            // String as JSON
            builder.Property(x => x.BookmarkPins)
                   .HasColumnType("NVARCHAR(MAX)");
            builder.Property(x => x.Status)
                   .HasDefaultValue(InstantCoachStatus.New);
        }
    }
}