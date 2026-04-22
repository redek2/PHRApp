using Microsoft.EntityFrameworkCore;
using PHRApp.Models.Entities;
using PHRApp.Data.Configurations;

namespace PHRApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Entry> Entries { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EntryConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
