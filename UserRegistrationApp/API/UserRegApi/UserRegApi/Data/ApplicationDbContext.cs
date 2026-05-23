using Microsoft.EntityFrameworkCore;
using UserRegApi.Models;

namespace UserRegApi.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {
            
        }
        public DbSet<User> Users { get; set; }
    }
}
