using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PHRApp.Models.Entities;

namespace PHRApp.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            // 1. Table
            builder.ToTable("Categories");

            // 2. Keys
            builder.HasKey(c => c.Id);

            // 3. Properties
            builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
            builder.Property(c => c.Description).HasMaxLength(1000);

            // 4. Indexes
            builder.HasIndex(c => c.Name).IsUnique();

            // 5. Relationships
            builder.HasMany(c => c.EntryCategories)
                   .WithOne(ec => ec.Category)
                   .HasForeignKey(ec => ec.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
