// Member 6 summary: sanctioned user and deciding administrator are int Identity references.
using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class Sanction : BaseEntity
{
    public int UserId { get; set; }
    public int DisputeId { get; set; }
    public Dispute Dispute { get; set; } = null!;
    public SanctionType SanctionType { get; set; }
    public int Days { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public int DecidedByAdminId { get; set; }
}

