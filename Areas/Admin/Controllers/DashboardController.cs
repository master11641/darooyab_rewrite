using System.Globalization;
using darooyab.Models;
using darooyab_rewrite.Areas.Admin.ViewModels;
using darooyab_rewrite.Services.Authentication;
using darooyab_rewrite.Services.Authentication.Pharmacy;
using darooyab_rewrite.Services.PharmacyRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace darooyab_rewrite.Areas.Admin.Controllers;

[Area("Admin")]
[Route("/PharmacyAdmin")]
[Authorize(Policy = PharmacyAuthenticationDefaults.AuthorizationPolicy)]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public sealed class DashboardController(
    darooConContext context,
    IAuthenticationService authenticationService,
    IPharmacyWalletService walletService) : Controller
{
    private const string PatientReferralCategoryType = "patient_referral";

    [HttpGet("Dashboard")]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var user = authenticationService.GetCurrentUser();
        if (user is null ||
            !int.TryParse(user.UserId, NumberStyles.None, CultureInfo.InvariantCulture, out var pharmacyAccountId))
        {
            return Forbid();
        }

        var now = DateTime.Now;
        var pharmacy = await context.PharmacyAccounts
            .AsNoTracking()
            .Where(account =>
                account.Id == pharmacyAccountId &&
                account.AccountStatus == "2" &&
                account.DarooyabAdminApproved != false &&
                (account.AccountExpireDate == null || account.AccountExpireDate >= now))
            .Select(account => new
            {
                account.PharmacyIdFk,
                PharmacyName = account.PharmacyIdFkNavigation.Name
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (pharmacy is null || string.IsNullOrWhiteSpace(pharmacy.PharmacyName))
        {
            await authenticationService.LogoutAsync();
            return Challenge();
        }

        var referralCategoryId = await context.Categories
            .AsNoTracking()
            .Where(category => category.Type == PatientReferralCategoryType)
            .Select(category => (int?)category.CatId)
            .SingleOrDefaultAsync(cancellationToken);
        if (!referralCategoryId.HasValue)
        {
            throw new InvalidOperationException(
                "The patient referral category is missing from the configured database.");
        }

        var today = DateTime.Today;
        var referralMetrics = await context.PharmacyRequests
            .AsNoTracking()
            .Where(request =>
                request.PharmacyAccountIdFk == pharmacyAccountId &&
                request.RequestTypeFromCategoryIdFk == referralCategoryId.Value &&
                request.RequestExpireDate.HasValue)
            .GroupBy(_ => 1)
            .Select(group => new
            {
                Total = group.Count(),
                Valid = group.Count(request => request.RequestExpireDate!.Value.Date >= today)
            })
            .SingleOrDefaultAsync(cancellationToken);

        var rankingMetrics = await context.RankingTransactions
            .AsNoTracking()
            .Where(ranking => ranking.PharmacyAccountId == pharmacyAccountId)
            .GroupBy(_ => 1)
            .Select(group => new
            {
                Total = group.Count(),
                Active = group.Count(ranking => ranking.StartDate <= now && ranking.EndDate >= now),
                AmountRial = group.Sum(ranking => (long)ranking.Amount)
            })
            .SingleOrDefaultAsync(cancellationToken);

        var prescriptionMetrics = await context.DarooyabPrescriptions
            .AsNoTracking()
            .Where(prescription => prescription.PharmacyId == pharmacy.PharmacyIdFk)
            .GroupBy(_ => 1)
            .Select(group => new
            {
                Total = group.Count(),
                Reviewed = group.Count(prescription => prescription.IsReviewed == true),
                Unreviewed = group.Count(prescription => prescription.IsReviewed != true)
            })
            .SingleOrDefaultAsync(cancellationToken);

        var balanceInRial = await walletService.GetBalanceInRialAsync(
            pharmacyAccountId,
            cancellationToken);
        var validReferralCount = referralMetrics?.Valid ?? 0;
        var totalPrescriptionCount = prescriptionMetrics?.Total ?? 0;

        ViewData["PharmacyName"] = pharmacy.PharmacyName;
        return View(new AdminDashboardViewModel(
            pharmacy.PharmacyName,
            user.UserName,
            validReferralCount,
            (referralMetrics?.Total ?? 0) - validReferralCount,
            rankingMetrics?.Total ?? 0,
            rankingMetrics?.Active ?? 0,
            (rankingMetrics?.AmountRial ?? 0) / 10,
            totalPrescriptionCount,
            prescriptionMetrics?.Reviewed ?? 0,
            prescriptionMetrics?.Unreviewed ?? 0,
            Math.Max(0, balanceInRial) / 10));
    }
}
