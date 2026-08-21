using FinBineBackend.UserGroupRegistration.Models;
using Microsoft.EntityFrameworkCore;

namespace FinBineBackend.UserGroupRegistration.Data
{
    public class GroupDbContext : DbContext
    {
        public GroupDbContext(DbContextOptions<GroupDbContext> options)
            : base(options)
        {
        }

        public DbSet<GroupDbRecord> Groups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GroupDbRecord>(entity =>
            {
                entity.ToTable("Groups");

                entity.HasKey(g => g.GroupId);

                entity.Property(g => g.GroupId)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(g => g.OwnerUserId)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(g => g.GroupType)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(g => g.CreatedAt)
                    .IsRequired();
            });
        }
    }
}
