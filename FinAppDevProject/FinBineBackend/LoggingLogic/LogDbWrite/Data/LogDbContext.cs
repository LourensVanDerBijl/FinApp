using FinBineBackend.LoggingLogic.LogDbWrite.Models;
using Microsoft.EntityFrameworkCore;

namespace FinBineBackend.LoggingLogic.LogDbWrite.Data
{
    public class LogDbContext : DbContext
    {
        public LogDbContext(DbContextOptions<LogDbContext> options)
            : base(options)
        {
        }

        public DbSet<LogDbRecord> Logs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LogDbRecord>(entity =>
            {
                entity.ToTable("Logs");

                entity.HasKey(log => log.Id);

                entity.Property(log => log.Id)
                    .ValueGeneratedNever();

                entity.Property(log => log.Timestamp)
                    .IsRequired();

                entity.Property(log => log.IngestionDate)
                    .IsRequired();

                // Speeds up future queries like "delete logs older than 60 days".
                entity.HasIndex(log => log.IngestionDate);

                entity.Property(log => log.Type)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(log => log.Source)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(log => log.Level)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(log => log.Message)
                    .IsRequired();
            });
        }
    }
}