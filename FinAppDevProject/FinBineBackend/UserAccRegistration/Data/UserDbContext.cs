using FinBineBackend.UserAccRegistration.Models;
using Microsoft.EntityFrameworkCore;

namespace FinBineBackend.UserAccRegistration.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserDbRecord> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserDbRecord>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(u => u.UserId);

                entity.Property(u => u.UserId)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(u => u.PreferName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(u => u.DateOfBirth)
                    .IsRequired();

                entity.Property(u => u.AccountType)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(u => u.GroupId)
                    .HasMaxLength(20);

                entity.Property(u => u.CreatedAt)
                    .IsRequired();
            });
        }
    }
}