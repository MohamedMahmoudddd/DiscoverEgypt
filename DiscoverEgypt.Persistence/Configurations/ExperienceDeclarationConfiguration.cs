// Member 6 summary: declarations are attributed to an int Identity host ID.
using DiscoverEgypt.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscoverEgypt.Persistence.Configurations;

public class ExperienceDeclarationConfiguration : IEntityTypeConfiguration<ExperienceDeclaration>
{
    public void Configure(EntityTypeBuilder<ExperienceDeclaration> builder)
    {
        builder.Property(x => x.HostUserId).IsRequired();
        builder.Property(x => x.DeclarationType).HasConversion<string>().HasMaxLength(40).IsRequired();
        builder.Property(x => x.DeclarationText).IsRequired().HasMaxLength(5000);
        builder.HasOne(x => x.Experience).WithMany(x => x.Declarations).HasForeignKey(x => x.ExperienceId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(x => x.HostUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

