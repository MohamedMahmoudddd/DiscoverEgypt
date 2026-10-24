// Member 6 summary: notification ownership uses int Identity IDs and supports a nullable int booking link.
using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class Notification : BaseEntity
{
    public int UserId { get; set; }
    public int? BookingId { get; set; }
    public Booking? Booking { get; set; }
    public NotificationType Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

