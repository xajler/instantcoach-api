using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
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
        public DbSet<BookmarkPinDbEntity> BookmarkPins { get; set; }

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

    public interface IAuditable
    {
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
    }

    public class InstantCoachDbEntity : Entity, IAuditable
    {
        public string Description { get; set; }
        public InstantCoachStatus Status { get; set; }
        public string TicketId { get; set; }
        public string Reference { get; set; }
        public int EvaluatorId { get; set; }
        public int AgentId { get; set; }
        public string EvaluatorName { get; set; }
        public string AgentName { get; set; }
        public string Comments { get; set; }
        public int CommentsCount { get; set; }
        public List<InstantCoachBookmarkPinDbEntity> InstantCoachBookmarkPins { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
    }

    public class BookmarkPinDbEntity : Entity
    {
        public int Index { get; set; }
        public int Range { get; set; }
        public string Comment { get; set; }
        public string MediaUrl { get; set; }
        public List<InstantCoachBookmarkPinDbEntity> InstantCoachBookmarkPins { get; set; }
    }

    public class InstantCoachBookmarkPinDbEntity
    {
        public int InstantCoachId { get; set; }
        public InstantCoachDbEntity InstantCoach { get; set; }
        public int BookmarkPinId { get; set; }
        public BookmarkPinDbEntity BookmarkPin { get; set; }
    }

    internal class InstantCoachConfiguration : IEntityTypeConfiguration<InstantCoachDbEntity>
    {
        public void Configure(EntityTypeBuilder<InstantCoachDbEntity> builder)
        {
            var statusConvert = new EnumToNumberConverter<InstantCoachStatus, byte>();
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .ForSqlServerUseSequenceHiLo("ic_hilo");
            builder.Property(x => x.Description)
                   .HasMaxLength(1000)
                   .IsRequired();
            builder.Property(x => x.Status)
                   .HasConversion(statusConvert)
                   .IsRequired();
            // builder.Property(x => x.Status)
            //        .HasDefaultValue(InstantCoachStatus.New);
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
            builder.Property(x => x.CommentsCount)
                   .IsRequired();
            // String as JSON
            builder.Property(x => x.Comments)
                   .HasColumnType("NVARCHAR(MAX)")
                   .IsRequired();
        }
    }

    internal class BookmarkPinConfiguration
        : IEntityTypeConfiguration<BookmarkPinDbEntity>
    {
        public void Configure(
            EntityTypeBuilder<BookmarkPinDbEntity> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .ForSqlServerUseSequenceHiLo("ic_hilo");
            builder.Property(x => x.Index)
                   .IsRequired();
            builder.Property(x => x.Range)
                   .IsRequired();
            builder.Property(x => x.MediaUrl)
                   .HasMaxLength(1000)
                   .IsRequired();
            builder.Property(x => x.Comment)
                   .HasColumnType("NVARCHAR(MAX)");
        }
    }

    internal class InstantCoachBookmarkPinConfiguration
        : IEntityTypeConfiguration<InstantCoachBookmarkPinDbEntity>
    {
        public void Configure(
            EntityTypeBuilder<InstantCoachBookmarkPinDbEntity> builder)
        {
            builder.HasKey(x =>
                new { x.InstantCoachId, x.BookmarkPinId });

            builder.HasOne(x => x.InstantCoach)
                   .WithMany(x => x.InstantCoachBookmarkPins)
                   .HasForeignKey(x => x.InstantCoachId);

            builder.HasOne(x => x.BookmarkPin)
                   .WithMany(x => x.InstantCoachBookmarkPins)
                   .HasForeignKey(x => x.BookmarkPinId);
        }
    }
}