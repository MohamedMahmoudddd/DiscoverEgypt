// Member 6 summary: host profile ownership references an int Identity user key.
using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Domain.Entities;

public class HostProfile : BaseEntity
{
    public int UserId { get; set; }
    public HostType HostType { get; set; }
    public string Bio { get; set; } = string.Empty;
    public string? ProfileImageKey { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Experience> Experiences { get; set; } = new List<Experience>();
}

