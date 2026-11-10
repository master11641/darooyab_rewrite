using darooyab.Models;
using darooyab_rewrite.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace darooyab_rewrite.ViewComponents;

public sealed class PublicRightPanelViewComponent(
    darooConContext context,
    ILogger<PublicRightPanelViewComponent> logger) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(int? drugId)
    {
        var model = new PublicRightPanelViewModel();

        try
        {
            var publicAdvertisements = await context.TblAdvertisements
                .AsNoTracking()
                .Where(ad => ad.AdvIsActive == true && ad.AdvIsPublic == true)
                .OrderByDescending(ad => ad.AdvRank)
                .Take(8)
                .Select(ad => new SidebarAdvertisement(
                    ad.AdvImageUrl,
                    ad.AdvImageAlt,
                    ad.AdvLink,
                    true))
                .ToListAsync();

            var advertisements = drugId.HasValue
                ? await context.TblAdvertisements
                    .AsNoTracking()
                    .Where(ad =>
                        ad.AdvIsActive == true &&
                        ad.AdvIsPublic == false &&
                        ad.AdvDrugIdFk == drugId)
                    .OrderByDescending(ad => ad.AdvRank)
                    .Take(8)
                    .Select(ad => new SidebarAdvertisement(
                        ad.AdvImageUrl,
                        ad.AdvImageAlt,
                        ad.AdvLink,
                        false))
                    .ToListAsync()
                : [];

            var latestArticles = await context.DarooyabAppNews
                .AsNoTracking()
                .Where(item => item.IsActive)
                .OrderByDescending(item => item.OrderIndex)
                .Take(5)
                .Select(item => new SidebarNewsItem(item.Id, item.Title, item.SubTitle))
                .ToListAsync();

            var latestNews = await context.News
                .AsNoTracking()
                .OrderByDescending(item => item.NewsId)
                .Take(5)
                .Select(item => new SidebarNewsItem(item.NewsId, item.Title, item.Summary))
                .ToListAsync();

            var newDrugRows = await context.DrugsNames
                .AsNoTracking()
                .Where(item =>
                    item.IsNewestDrug == true &&
                    item.DrugNameLanguage == false &&
                    item.Irc != null)
                .OrderByDescending(item => item.Id)
                .Take(20)
                .Select(item => new DrugNameRow(item.DrugId, item.DrugName, item.Irc))
                .ToListAsync();

            var frequentlyUsedRows = await context.DrugsNames
                .AsNoTracking()
                .Where(item =>
                    item.AddDate.HasValue &&
                    item.AddDate < 1000 &&
                    item.DrugNameLanguage == false)
                .OrderBy(item => item.AddDate)
                .Take(20)
                .Select(item => new DrugNameRow(item.DrugId, item.DrugName))
                .ToListAsync();

            var drugIds = newDrugRows
                .Concat(frequentlyUsedRows)
                .Select(item => item.DrugId)
                .Distinct()
                .ToArray();

            var genericNames = await context.DrugsNames
                .AsNoTracking()
                .Where(item => drugIds.Contains(item.DrugId) && item.DrugNameType == "A")
                .Select(item => new { item.DrugId, item.DrugName })
                .ToListAsync();

            var genericNamesById = genericNames
                .GroupBy(item => item.DrugId)
                .ToDictionary(group => group.Key, group => group.First().DrugName ?? string.Empty);
            var newestDrugIrcs = newDrugRows
                .Where(item => item.Irc.HasValue)
                .Select(item => item.Irc!.Value)
                .Distinct()
                .ToArray();
            var englishBrandNames = await context.DrugsNames
                .AsNoTracking()
                .Where(item =>
                    item.DrugNameLanguage == true &&
                    item.Irc.HasValue &&
                    newestDrugIrcs.Contains(item.Irc.Value))
                .Select(item => new { Irc = item.Irc!.Value, item.DrugName })
                .ToListAsync();
            var englishBrandNamesByIrc = englishBrandNames
                .GroupBy(item => item.Irc)
                .ToDictionary(group => group.Key, group => group.First().DrugName ?? string.Empty);

            model = new PublicRightPanelViewModel
            {
                PublicAdvertisements = publicAdvertisements,
                Advertisements = advertisements,
                LatestArticles = latestArticles,
                LatestNews = latestNews,
                NewDrugs = ToNewestSidebarItems(newDrugRows, englishBrandNamesByIrc),
                FrequentlyUsedDrugs = ToSidebarItems(frequentlyUsedRows, genericNamesById)
            };
        }
        catch (SqlException exception)
        {
            logger.LogError(
                "Could not load public right-panel data from SQL Server. SQL error number: {SqlErrorNumber}.",
                exception.Number);
            model.DataUnavailable = true;
        }

        return View(model);
    }

    private static IReadOnlyList<SidebarDrugItem> ToSidebarItems(
        IEnumerable<DrugNameRow> drugRows,
        IReadOnlyDictionary<int, string> genericNamesById)
    {
        var result = new List<SidebarDrugItem>();
        var seenDrugIds = new HashSet<int>();

        foreach (var row in drugRows)
        {
            var currentDrugId = row.DrugId;
            var persianName = row.DrugName;
            if (string.IsNullOrWhiteSpace(persianName) ||
                !seenDrugIds.Add(currentDrugId) ||
                !genericNamesById.TryGetValue(currentDrugId, out var englishName) ||
                string.IsNullOrWhiteSpace(englishName))
            {
                continue;
            }

            result.Add(new SidebarDrugItem(currentDrugId, persianName, englishName));
            if (result.Count == 5)
            {
                break;
            }
        }

        return result;
    }

    private static IReadOnlyList<SidebarDrugItem> ToNewestSidebarItems(
        IEnumerable<DrugNameRow> drugRows,
        IReadOnlyDictionary<long, string> englishBrandNamesByIrc)
    {
        var result = new List<SidebarDrugItem>();
        var seenDrugIds = new HashSet<int>();

        foreach (var row in drugRows)
        {
            if (string.IsNullOrWhiteSpace(row.DrugName) ||
                !seenDrugIds.Add(row.DrugId) ||
                !row.Irc.HasValue ||
                !englishBrandNamesByIrc.TryGetValue(row.Irc.Value, out var englishBrandName) ||
                string.IsNullOrWhiteSpace(englishBrandName))
            {
                continue;
            }

            result.Add(new SidebarDrugItem(row.DrugId, row.DrugName, englishBrandName, row.Irc));
            if (result.Count == 5)
            {
                break;
            }
        }

        return result;
    }

    private sealed record DrugNameRow(int DrugId, string? DrugName, long? Irc = null);
}
