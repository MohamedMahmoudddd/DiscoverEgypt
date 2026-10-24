using System.Security.Claims;
using DiscoverEgypt.Application.Notifications;
using Microsoft.AspNetCore.Mvc;

namespace DiscoverEgypt.Presentation.ViewComponents;

/// <summary>Member 6 summary: navbar bell displays the signed-in user's unread notification count.</summary>
public sealed class NotificationBellViewComponent(INotificationService notifications) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (UserClaimsPrincipal.Identity?.IsAuthenticated != true || !int.TryParse(UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Content(string.Empty);
        return View(await notifications.GetUnreadCountAsync(userId));
    }
}
