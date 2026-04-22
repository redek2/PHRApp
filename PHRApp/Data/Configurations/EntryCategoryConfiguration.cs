using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PHRApp.Models.JoinEntities;

namespace PHRApp.Data.Configurations
{
    public class EntryCategoryConfiguration : IEntityTypeConfiguration<EntryCategory>
    {
        public void Configure(EntityTypeBuilder<EntryCategory> builder)
        {
            // 1. Table
            builder.ToTable("EntryCategories");

            // 2. Keys
            builder.HasKey(ec => new { ec.EntryId, ec.CategoryId });

            // 3. Relationships
            builder.HasOne(ec => ec.Category)
                   .WithMany(c => c.EntryCategories)
                   .HasForeignKey(ec => ec.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}