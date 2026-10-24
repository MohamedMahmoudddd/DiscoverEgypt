// Member 6 summary: private evidence now records pending/approved/rejected review state, reviewer, decision time, and note.
using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class ExperienceEvidence : BaseEntity
{
    public int ExperienceId { get; set; }
    public Experience Experience { get; set; } = null!;
    public EvidenceKind EvidenceKind { get; set; }
    public string? TextContent { get; set; }
    public string? StorageKey { get; set; }
    public EvidenceReviewStatus ReviewStatus { get; set; } = EvidenceReviewStatus.Pending;
    public int? ReviewedByAdminId { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? ReviewNote { get; set; }
    public DateTime UploadedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}

