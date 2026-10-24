using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums;
using DiscoverEgypt.Application.Notifications;
using DiscoverEgypt.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Infrastructure.Notifications;

/// <summary>Member 6 summary: persists notifications and only returns or changes records owned by the requested integer user ID.</summary>
public sealed class NotificationService(DiscoverEgyptDbContext db) : INotificationService
{
    public async Task<IReadOnlyList<Notification>> GetLatestAsync(int userId, int limit = 50, CancellationToken cancellationToken = default) =>
        await db.Notifications.AsNoTracking().Where(x => x.UserId == userId).OrderByDescending(x => x.CreatedAt).Take(Math.Clamp(limit, 1, 100)).ToListAsync(cancellationToken);

    public Task<int> GetUnreadCountAsync(int userId, CancellationToken cancellationToken = default) =>
        db.Notifications.CountAsync(x => x.UserId == userId && !x.IsRead, cancellationToken);

    public async Task<bool> MarkReadAsync(int userId, int notificationId, CancellationToken cancellationToken = default)
    {
        var item = await db.Notifications.FirstOrDefaultAsync(x => x.Id == notificationId && x.UserId == userId, cancellationToken);
        if (item is null) return false;
        item.IsRead = true;
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task MarkAllReadAsync(int userId, CancellationToken cancellationToken = default)
    {
        await db.Notifications.Where(x => x.UserId == userId && !x.IsRead).ExecuteUpdateAsync(setters => setters.SetProperty(x => x.IsRead, true), cancellationToken);
    }

    public async Task CreateAsync(int userId, NotificationType type, string message, int? bookingId = null, CancellationToken cancellationToken = default)
    {
        db.Notifications.Add(new Notification { UserId = userId, Type = type, Message = message, BookingId = bookingId, CreatedAt = DateTime.UtcNow });
        await db.SaveChangesAsync(cancellationToken);
    }
}
