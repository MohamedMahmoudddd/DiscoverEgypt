// Member 6 summary: reviewer is an optional int Identity user ID while pending.
using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class VerificationRequest : BaseEntity
{
    public int GuideProfileId { get; set; }
    public GuideProfile GuideProfile { get; set; } = null!;
    public VerificationStatus Status { get; set; }
    public DateOnly LicenseExpiryDate { get; set; }
    public string? RejectionReason { get; set; }
    public int? ReviewedByAdminId { get; set; }
    public DateTime SubmittedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public ICollection<VerificationDocument> Documents { get; set; } = new List<VerificationDocument>();
}

