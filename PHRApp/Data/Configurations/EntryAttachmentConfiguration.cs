using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PHRApp.Models.JoinEntities;

namespace PHRApp.Data.Configurations
{
    public class EntryAttachmentConfiguration : IEntityTypeConfiguration<EntryAttachment>
    {
        public void Configure(EntityTypeBuilder<EntryAttachment> builder)
        {
            // 1. Table
            builder.ToTable("EntryAttachments");

            // 2. Keys
            builder.HasKey(ea => new { ea.EntryId, ea.AttachmentId });

            // 3. Relationships
            builder.HasOne(ea => ea.Attachment)
                   .WithMany(a => a.EntryAttachments)
                   .HasForeignKey(ea => ea.AttachmentId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}