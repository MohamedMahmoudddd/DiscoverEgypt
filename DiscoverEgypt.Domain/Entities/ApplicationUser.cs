using Microsoft.AspNetCore.Identity;

namespace DiscoverEgypt.Domain.Entities;

/// <summary>Member 6: Identity users use integer identity keys, matching the domain identifier standard.</summary>
public class ApplicationUser : IdentityUser<int>
{
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
