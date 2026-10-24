// Member 6 summary: evidence has an audited administrator review state before its parent experience can publish.
using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverEgypt.Persistence.Configurations;

public class ExperienceEvidenceConfiguration : IEntityTypeConfiguration<ExperienceEvidence>
{
    public void Configure(EntityTypeBuilder<ExperienceEvidence> builder)
    {
        builder.Property(x => x.EvidenceKind).HasConversion<string>().HasMaxLength(40).IsRequired();
        builder.Property(x => x.TextContent).HasMaxLength(5000);
        builder.Property(x => x.StorageKey).HasMaxLength(500);
        builder.Property(x => x.ReviewStatus).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(x => x.ReviewNote).HasMaxLength(1000);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.ReviewedByAdminId).OnDelete(DeleteBehavior.Restrict);
        builder.ToTable(t => t.HasCheckConstraint("CK_ExperienceEvidence_HasContent", "[TextContent] IS NOT NULL OR [StorageKey] IS NOT NULL"));
        builder.HasOne(x => x.Experience).WithMany(x => x.Evidence).HasForeignKey(x => x.ExperienceId).OnDelete(DeleteBehavior.Cascade);
    }
}

