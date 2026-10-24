using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums;

namespace DiscoverEgypt.Application.Notifications;

/// <summary>Member 6 summary: user-scoped notification retrieval, unread counts, read updates, and notification creation contract.</summary>
public interface INotificationService
{
    Task<IReadOnlyList<Notification>> GetLatestAsync(int userId, int limit = 50, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> MarkReadAsync(int userId, int notificationId, CancellationToken cancellationToken = default);
    Task MarkAllReadAsync(int userId, CancellationToken cancellationToken = default);
    Task CreateAsync(int userId, NotificationType type, string message, int? bookingId = null, CancellationToken cancellationToken = default);
}
