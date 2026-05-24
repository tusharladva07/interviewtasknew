using Microsoft.EntityFrameworkCore;
using UserRegApi.Models;

namespace UserRegApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Name).HasMaxLength(100).IsRequired();
                entity.Property(u => u.Email).HasMaxLength(256).IsRequired();
                entity.Property(u => u.Password).IsRequired();
                entity.Property(u => u.Gender).HasMaxLength(20).IsRequired();
                entity.Property(u => u.Hobby).HasMaxLength(50).IsRequired();
            });
        }
    }
}
