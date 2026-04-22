using Microsoft.EntityFrameworkCore;
using PHRApp.Data.Configurations;
using PHRApp.Models.Entities;
using PHRApp.Models.JoinEntities;

namespace PHRApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Entry> Entries { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Attachment> Attachments { get; set; } = null!;
        public DbSet<EntryCategory> EntryCategories { get; set; } = null!;
        public DbSet<EntryAttachment> EntryAttachments { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new EntryConfiguration());
            modelBuilder.ApplyConfiguration(new EntryCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new EntryAttachmentConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=phrapp.db");
            }
        }
    }
}