using LinguaSmart.Models;
using Microsoft.EntityFrameworkCore;

namespace LinguaSmart.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Word> Words { get; set; }
    }
}
