namespace DiscoverEgypt.Presentation.Models;

/// <summary>Member 6 summary: dashboard totals expose admin workload, platform activity, payment volume, and unresolved disputes.</summary>
public sealed class AdminDashboardViewModel
{
    public int Users { get; init; }
    public int ActiveUsers { get; init; }
    public int PendingVerifications { get; init; }
    public int PendingExperiences { get; init; }
    public int Bookings { get; init; }
    public int OpenDisputes { get; init; }
    public decimal GrossPayments { get; init; }
    public decimal PendingPayouts { get; init; }
}
