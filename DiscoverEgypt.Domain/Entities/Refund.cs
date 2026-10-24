// Member 6 summary: administrator identity on refund decisions uses a nullable int Identity ID.
using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class Refund : BaseEntity
{
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    public int? DisputeId { get; set; }
    public Dispute? Dispute { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
    public int? DecidedByAdminId { get; set; }
    public DateTime CreatedAt { get; set; }
}

