using System.Security.Claims;
using DiscoverEgypt.Application.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.Controllers;

/// <summary>Member 6 summary: authenticated notification inbox with owner-scoped mark-read actions.</summary>
[Authorize]
public sealed class NotificationsController(INotificationService notifications) : Controller
{
    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index(CancellationToken cancellationToken) =>
        View(await notifications.GetLatestAsync(CurrentUserId, 100, cancellationToken));

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkRead(int id, CancellationToken cancellationToken) =>
        await notifications.MarkReadAsync(CurrentUserId, id, cancellationToken) ? RedirectToAction(nameof(Index)) : NotFound();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAllRead(CancellationToken cancellationToken)
    {
        await notifications.MarkAllReadAsync(CurrentUserId, cancellationToken);
        return RedirectToAction(nameof(Index));
    }
}
