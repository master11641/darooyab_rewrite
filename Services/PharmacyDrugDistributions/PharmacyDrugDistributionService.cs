using darooyab.Models;
using darooyab_rewrite.Areas.Admin.ViewModels;
using darooyab_rewrite.Utilities;
using Ganss.Xss;
using Microsoft.EntityFrameworkCore;

namespace darooyab_rewrite.Services.PharmacyDrugDistributions;

public sealed class PharmacyDrugDistributionService(
    darooConContext context) : IPharmacyDrugDistributionService
{
    private const int PageSize = 5;

    public async Task<PharmacyDrugDistributionPageViewModel> GetPageAsync(
        string? drugName,
        int factoryId,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var distributionsQuery = context.DarooyabDrugDistributions
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrEmpty(drugName))
        {
            var matchingDrugIds = context.DrugsNames
                .AsNoTracking()
                .Where(drug => drug.DrugName != null && drug.DrugName.Contains(drugName))
                .Select(drug => drug.Id);
            distributionsQuery = distributionsQuery
                .Where(item => matchingDrugIds.Contains(item.DrugsNameId));
        }

        if (factoryId > 0)
        {
            distributionsQuery = distributionsQuery
                .Where(item => item.FactoryId == factoryId);
        }

        var totalCount = await distributionsQuery.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)PageSize));
        var currentPage = Math.Clamp(page, 1, totalPages);
        var rows = await distributionsQuery
            .OrderByDescending(item => item.Id)
            .Skip((currentPage - 1) * PageSize)
            .Take(PageSize)
            .Select(item => new
            {
                item.Id,
                item.AnnouncementDate,
                item.Description,
                DrugName = item.DrugsName == null ? null : item.DrugsName.DrugName,
                FactoryName = item.Factory == null ? null : item.Factory.FactoryName
            })
            .ToListAsync(cancellationToken);

        var sanitizer = new HtmlSanitizer();
        var factories = await context.Factorys
            .AsNoTracking()
            .Where(factory => factory.FactoryTypeId == 4)
            .Select(factory => new PharmacyDistributionFactoryItem(
                factory.FactoryId,
                factory.FactoryName ?? string.Empty))
            .ToListAsync(cancellationToken);

        return new PharmacyDrugDistributionPageViewModel
        {
            Rows = rows
                .Select(item => new PharmacyDrugDistributionRow(
                    item.Id,
                    item.DrugName ?? string.Empty,
                    item.FactoryName ?? string.Empty,
                    PersianDateFormatter.Format(item.AnnouncementDate),
                    sanitizer.Sanitize(item.Description ?? string.Empty)))
                .ToArray(),
            Factories = factories,
            DrugName = drugName ?? string.Empty,
            FactoryId = factoryId,
            Page = currentPage,
            TotalPages = totalPages
        };
    }
}
