using DiscoverEgypt.Domain.Entities;
using DiscoverEgypt.Domain.Enums;
using DiscoverEgypt.Application.Notifications;
using DiscoverEgypt.Persistence;
using DiscoverEgypt.Presentation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DiscoverEgypt.Presentation.Controllers;

/// <summary>Member 6 summary: role-protected platform dashboard and operational views/actions for accounts, verification, listings, bookings, ledger, and disputes.</summary>
[Authorize(Roles = "Admin")]
public sealed class AdminController(DiscoverEgyptDbContext db, UserManager<ApplicationUser> users, INotificationService notifications) : Controller
{
    public async Task<IActionResult> Index()
    {
        var model = new AdminDashboardViewModel
        {
            Users = await db.Users.CountAsync(),
            ActiveUsers = await db.Users.CountAsync(x => x.IsActive),
            PendingVerifications = await db.VerificationRequests.CountAsync(x => x.Status == VerificationStatus.Pending),
            PendingExperiences = await db.Experiences.CountAsync(x => x.Status == ExperienceStatus.PendingApproval),
            Bookings = await db.Bookings.CountAsync(),
            OpenDisputes = await db.Disputes.CountAsync(x => x.Status == DisputeStatus.Open || x.Status == DisputeStatus.UnderReview),
            GrossPayments = await db.Payments.Where(x => x.Status == PaymentStatus.Paid).SumAsync(x => (decimal?)x.Amount) ?? 0,
            PendingPayouts = await db.Payouts.Where(x => x.Status == PayoutStatus.Pending).SumAsync(x => (decimal?)x.Amount) ?? 0
        };
        return View(model);
    }

    public async Task<IActionResult> UsersList() => View("Users", await db.Users.OrderByDescending(x => x.CreatedAt).ToListAsync());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SetUserActive(int id, bool active)
    {
        var user = await users.FindByIdAsync(id.ToString());
        if (user is null) return NotFound();
        if (!active && user.Id.ToString() == users.GetUserId(User)) return BadRequest("لا يمكن تعطيل الحساب الإداري المستخدم حاليًا.");
        user.IsActive = active;
        if (!active)
        {
            var host = await db.HostProfiles.Include(x => x.Experiences).FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (host is not null)
                foreach (var experience in host.Experiences.Where(x => x.Status == ExperienceStatus.Published)) experience.Status = ExperienceStatus.Unpublished;
        }
        var update = await users.UpdateAsync(user);
        if (!update.Succeeded) return BadRequest(string.Join("; ", update.Errors.Select(x => x.Description)));
        return RedirectToAction(nameof(UsersList));
    }

    public async Task<IActionResult> Verifications() => View(await db.VerificationRequests
        .Include(x => x.GuideProfile).Include(x => x.Documents)
        .Where(x => x.Status == VerificationStatus.Pending).OrderBy(x => x.SubmittedAt).ToListAsync());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ReviewVerification(int id, bool approve, string? reason)
    {
        var request = await db.VerificationRequests.Include(x => x.GuideProfile).FirstOrDefaultAsync(x => x.Id == id);
        if (request is null) return NotFound();
        if (request.Status != VerificationStatus.Pending) return BadRequest("طلب التحقق ليس قيد الانتظار.");
        if (!approve && string.IsNullOrWhiteSpace(reason)) return BadRequest("سبب الرفض مطلوب.");
        if (reason?.Length > 1000) return BadRequest("سبب الرفض يجب ألا يتجاوز 1000 حرف.");
        request.Status = approve ? VerificationStatus.Approved : VerificationStatus.Rejected;
        request.RejectionReason = approve ? null : reason!.Trim();
        request.ReviewedByAdminId = int.Parse(users.GetUserId(User)!);
        request.ReviewedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        await notifications.CreateAsync(request.GuideProfile.UserId, NotificationType.VerificationUpdate,
            approve ? "تم اعتماد طلب التحقق الخاص بك." : $"تم رفض طلب التحقق: {request.RejectionReason}");
        return RedirectToAction(nameof(Verifications));
    }

    public async Task<IActionResult> Experiences() => View(await db.Experiences
        .Include(x => x.HostProfile).Include(x => x.ExperienceType).Include(x => x.Media).Include(x => x.Evidence)
        .Where(x => x.Status == ExperienceStatus.PendingApproval || x.Status == ExperienceStatus.Published || x.Status == ExperienceStatus.Unpublished)
        .OrderBy(x => x.Status).ThenBy(x => x.CreatedAt).ToListAsync());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ReviewExperience(int id, bool approve, string? reason)
    {
        var experience = await db.Experiences.Include(x => x.Media).Include(x => x.Evidence).FirstOrDefaultAsync(x => x.Id == id);
        if (experience is null) return NotFound();
        var images = experience.Media.Where(x => x.MediaType == MediaType.Image).ToList();
        if (approve && (images.Count == 0 || images.Count(x => x.IsCover) != 1)) return BadRequest("يجب إضافة صورة واحدة على الأقل وتحديد صورة غلاف صالحة قبل النشر.");
        if (approve && (experience.Evidence.Count == 0 || experience.Evidence.Any(x => x.ReviewStatus != EvidenceReviewStatus.Approved))) return BadRequest("يجب مراجعة واعتماد كل أدلة التجربة قبل نشرها.");
        if (!approve && string.IsNullOrWhiteSpace(reason)) return BadRequest("سبب الرفض مطلوب.");
        if (reason?.Length > 1000) return BadRequest("سبب الرفض يجب ألا يتجاوز 1000 حرف.");
        experience.Status = approve ? ExperienceStatus.Published : ExperienceStatus.Rejected;
        experience.RejectionReason = approve ? null : reason!.Trim();
        experience.ReviewedByAdminId = int.Parse(users.GetUserId(User)!);
        experience.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        var hostUserId = await db.HostProfiles.Where(x => x.Id == experience.HostProfileId).Select(x => x.UserId).SingleAsync();
        await notifications.CreateAsync(hostUserId, NotificationType.ExperienceApprovalUpdate,
            approve ? $"تم اعتماد تجربتك «{experience.Title}» ونشرها." : $"تم رفض تجربتك «{experience.Title}»: {experience.RejectionReason}");
        return RedirectToAction(nameof(Experiences));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UnpublishExperience(int id)
    {
        var experience = await db.Experiences.FirstOrDefaultAsync(x => x.Id == id);
        if (experience is null) return NotFound();
        if (experience.Status != ExperienceStatus.Published) return BadRequest("يمكن إلغاء نشر التجارب المنشورة فقط.");
        experience.Status = ExperienceStatus.Unpublished;
        experience.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Experiences));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ReviewEvidence(int id, bool approve, string? note)
    {
        var evidence = await db.ExperienceEvidence.Include(x => x.Experience).FirstOrDefaultAsync(x => x.Id == id);
        if (evidence is null) return NotFound();
        if (evidence.ReviewStatus != EvidenceReviewStatus.Pending) return BadRequest("تمت مراجعة هذا الدليل بالفعل.");
        if (!approve && string.IsNullOrWhiteSpace(note)) return BadRequest("سبب رفض الدليل مطلوب.");
        evidence.ReviewStatus = approve ? EvidenceReviewStatus.Approved : EvidenceReviewStatus.Rejected;
        evidence.ReviewedByAdminId = int.Parse(users.GetUserId(User)!);
        evidence.ReviewedAt = DateTime.UtcNow;
        evidence.ReviewNote = string.IsNullOrWhiteSpace(note) ? null : note.Trim()[..Math.Min(1000, note.Trim().Length)];
        await db.SaveChangesAsync();
        var hostUserId = await db.HostProfiles.Where(x => x.Id == evidence.Experience.HostProfileId).Select(x => x.UserId).SingleAsync();
        await notifications.CreateAsync(hostUserId, NotificationType.ExperienceApprovalUpdate,
            approve ? $"تم اعتماد دليل التحقق للتجربة «{evidence.Experience.Title}»." : $"رُفض أحد أدلة التجربة «{evidence.Experience.Title}»: {evidence.ReviewNote}");
        return RedirectToAction(nameof(Experiences));
    }

    public async Task<IActionResult> Bookings() => View(await db.Bookings.Include(x => x.Payment).Include(x => x.Payout).OrderByDescending(x => x.CreatedAt).Take(250).ToListAsync());
    public async Task<IActionResult> Transactions() => View(await db.Payments.Include(x => x.Booking).OrderByDescending(x => x.CreatedAt).Take(250).ToListAsync());
    public async Task<IActionResult> Payouts() => View(await db.Payouts.Include(x => x.Booking).OrderByDescending(x => x.EligibleAt).Take(250).ToListAsync());
    public async Task<IActionResult> Disputes() => View(await db.Disputes.Include(x => x.Booking).OrderBy(x => x.Status).ThenByDescending(x => x.CreatedAt).ToListAsync());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ResolveDispute(int id, DisputeOutcome outcome, string note)
    {
        var dispute = await db.Disputes.Include(x => x.Booking).ThenInclude(x => x.Payment)
            .Include(x => x.Booking).ThenInclude(x => x.Payout).FirstOrDefaultAsync(x => x.Id == id);
        if (dispute is null) return NotFound();
        if (dispute.Status is DisputeStatus.Resolved or DisputeStatus.Closed) return BadRequest("تم حسم النزاع بالفعل.");
        if (string.IsNullOrWhiteSpace(note)) return BadRequest("ملاحظة القرار مطلوبة.");
        if (note.Length > 3000) return BadRequest("ملاحظة القرار يجب ألا تتجاوز 3000 حرف.");
        var booking = dispute.Booking;
        var decidedBy = int.Parse(users.GetUserId(User)!);
        var now = DateTime.UtcNow;
        if (outcome == DisputeOutcome.ProviderFault)
        {
            if (booking.ServiceAmount > 0)
                db.Refunds.Add(new Refund { BookingId = booking.Id, DisputeId = dispute.Id, Amount = booking.ServiceAmount, Reason = note.Trim()[..Math.Min(1000, note.Trim().Length)], DecidedByAdminId = decidedBy, CreatedAt = now });
            if (booking.Payment is not null) booking.Payment.Status = PaymentStatus.Refunded;
            if (booking.Payout is not null) booking.Payout.Status = PayoutStatus.Cancelled;
            db.Sanctions.Add(new Sanction { UserId = booking.ProviderUserId, DisputeId = dispute.Id, SanctionType = SanctionType.ProviderSuspension, Days = 14, StartsAt = now, EndsAt = now.AddDays(14), DecidedByAdminId = decidedBy });
        }
        else if (outcome == DisputeOutcome.TouristFault)
        {
            db.Sanctions.Add(new Sanction { UserId = booking.TouristUserId, DisputeId = dispute.Id, SanctionType = SanctionType.TouristBan, Days = 30, StartsAt = now, EndsAt = now.AddDays(30), DecidedByAdminId = decidedBy });
        }
        dispute.Status = DisputeStatus.Resolved;
        dispute.Outcome = outcome;
        dispute.ResolutionNote = note.Trim();
        dispute.ResolvedByAdminId = decidedBy;
        dispute.ResolvedAt = now;
        await db.SaveChangesAsync();
        await notifications.CreateAsync(booking.TouristUserId, NotificationType.DisputeUpdate, $"تم حسم النزاع #{dispute.Id}: {outcome}.", booking.Id);
        await notifications.CreateAsync(booking.ProviderUserId, NotificationType.DisputeUpdate, $"تم حسم النزاع #{dispute.Id}: {outcome}.", booking.Id);
        return RedirectToAction(nameof(Disputes));
    }
}
