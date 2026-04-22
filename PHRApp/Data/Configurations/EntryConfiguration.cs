using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PHRApp.Models.Entities;
using PHRApp.Models.Enums;

namespace PHRApp.Data.Configurations
{
    public class EntryConfiguration : IEntityTypeConfiguration<Entry>
    {
        public void Configure(EntityTypeBuilder<Entry> builder)
        {
            // 1. Table
            builder.ToTable("Entries");

            // 2. Keys
            builder.HasKey(e => e.Id);

            // 3. Properties
            builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Description).HasMaxLength(4000);
            builder.Property(e => e.EventDate).IsRequired();
            builder.Property(e => e.Status).IsRequired().HasConversion<int>().HasDefaultValue(EntryStatus.Planned);
            builder.Property(e => e.IsArchived).IsRequired().HasDefaultValue(false);
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.UpdatedAt).IsRequired();

            // 4. Indexes
            builder.HasIndex(e => e.Title);
            builder.HasIndex(e => e.EventDate);

            // 5. Relationships
            builder.HasMany(e => e.EntryCategories)
                   .WithOne(ec => ec.Entry)
                   .HasForeignKey(ec => ec.EntryId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.EntryAttachments)
                   .WithOne(ea => ea.Entry)
                   .HasForeignKey(ea => ea.EntryId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}