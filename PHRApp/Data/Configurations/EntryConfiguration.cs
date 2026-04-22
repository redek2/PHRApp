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
            builder.ToTable("Entries");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Description).HasMaxLength(4000);
            builder.Property(e => e.EventDate).IsRequired();
            builder.Property(e => e.Status).IsRequired().HasConversion<int>().HasDefaultValue(EntryStatus.Planned);
            builder.Property(e => e.IsArchived).IsRequired().HasDefaultValue(false);
            builder.Property(e => e.CreatedAt).IsRequired();
            builder.Property(e => e.UpdatedAt).IsRequired();
        }
    }
}
