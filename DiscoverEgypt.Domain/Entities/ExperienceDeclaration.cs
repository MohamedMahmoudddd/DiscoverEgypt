// Member 6 summary: declaration host identity uses int and retains an auditable acceptance record.
using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class ExperienceDeclaration : BaseEntity
{
    public int ExperienceId { get; set; }
    public Experience Experience { get; set; } = null!;
    public int HostUserId { get; set; }
    public DeclarationType DeclarationType { get; set; }
    public string DeclarationText { get; set; } = string.Empty;
    public DateTime AcceptedAt { get; set; }
}

