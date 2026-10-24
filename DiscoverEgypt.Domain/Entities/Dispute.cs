// Member 6 summary: dispute opener and resolving administrator identifiers now use int Identity IDs.
using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class Dispute : BaseEntity
{
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    public int OpenedByUserId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DisputeStatus Status { get; set; }
    public DisputeOutcome? Outcome { get; set; }
    public string? ResolutionNote { get; set; }
    public int? ResolvedByAdminId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public ICollection<Refund> Refunds { get; set; } = new List<Refund>();
    public ICollection<Sanction> Sanctions { get; set; } = new List<Sanction>();
}

