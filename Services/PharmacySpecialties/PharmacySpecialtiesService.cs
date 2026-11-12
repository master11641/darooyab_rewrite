using System.Data;
using darooyab.Models;
using darooyab_rewrite.Areas.Admin.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace darooyab_rewrite.Services.PharmacySpecialties;

public sealed class PharmacySpecialtiesService(darooConContext context)
{
    public async Task<PharmacySpecialtiesPageViewModel?> GetPageAsync(
        int pharmacyAccountId,
        int activeTab,
        CancellationToken cancellationToken = default)
    {
        var pharmacyId = await context.PharmacyAccounts
            .AsNoTracking()
            .Where(account => account.Id == pharmacyAccountId)
            .Select(account => (int?)account.PharmacyIdFk)
            .SingleOrDefaultAsync(cancellationToken);
        if (!pharmacyId.HasValue)
        {
            return null;
        }

        var pharmacy = await context.Pharmacies
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == pharmacyId.Value, cancellationToken);
        if (pharmacy is null)
        {
            return null;
        }

        var specialtyOptions = await context.PharmacySpecialties
            .AsNoTracking()
            .Where(item => !item.IsDeleted && item.IsActive)
            .OrderBy(item => item.OrderIndex)
            .Select(item => new { item.ID, item.Title })
            .ToListAsync(cancellationToken);
        var selectedSpecialtyIds = await context.PharmacySpecialtyMaps
            .AsNoTracking()
            .Where(item =>
                item.PharmacyId_fk == pharmacy.Id &&
                !item.IsDeleted &&
                item.IsActive)
            .Select(item => item.PharmacySpecialtyId_fk)
            .ToArrayAsync(cancellationToken);
        var providerOptions = await context.InsuranceProviders
            .AsNoTracking()
            .Where(item => !item.IsDeleted && item.IsActive)
            .OrderBy(item => item.OrderIndex)
            .ThenBy(item => item.Name)
            .Select(item => new { item.Id, item.Name, item.InsuranceType })
            .ToListAsync(cancellationToken);
        var selectedProviderIds = await context.PharmacyInsuranceContracts
            .AsNoTracking()
            .Where(item =>
                item.PharmacyIdFk == pharmacy.Id &&
                !item.IsDeleted &&
                item.IsActive)
            .Select(item => item.InsuranceProviderIdFk)
            .ToArrayAsync(cancellationToken);
        var districts = await context.DarooyabCityDistricts
            .AsNoTracking()
            .Where(item => item.IsActive && item.CityId == pharmacy.CityIdFk)
            .OrderBy(item => item.OrderIndex)
            .ThenBy(item => item.Name)
            .Select(item => new { item.Id, item.Name })
            .ToListAsync(cancellationToken);
        var provinceName = await context.Categories
            .AsNoTracking()
            .Where(category => category.CatId == pharmacy.ProvinceIdFk)
            .Select(category => category.Name)
            .SingleOrDefaultAsync(cancellationToken);
        var cityName = await context.Categories
            .AsNoTracking()
            .Where(category => category.CatId == pharmacy.CityIdFk)
            .Select(category => category.Name)
            .SingleOrDefaultAsync(cancellationToken);

        return new PharmacySpecialtiesPageViewModel
        {
            PharmacyName = pharmacy.Name,
            ActiveTab = activeTab,
            ShowDistrictTab = districts.Count > 0,
            District = new PharmacyDistrictTabViewModel
            {
                DistrictId = pharmacy.DistrictId,
                Options = districts
                    .Select(item => new PharmacySelectionOptionViewModel(
                        item.Id,
                        item.Name,
                        item.Id == pharmacy.DistrictId))
                    .ToArray()
            },
            Specialties = new PharmacySpecialtyTabViewModel
            {
                SelectedIds = selectedSpecialtyIds,
                Options = specialtyOptions
                    .Select(item => new PharmacySelectionOptionViewModel(
                        item.ID,
                        item.Title,
                        selectedSpecialtyIds.Contains(item.ID)))
                    .ToArray()
            },
            Location = new PharmacyLocationTabViewModel
            {
                Latitude = pharmacy.Latitude,
                Longitude = pharmacy.Longitude
            },
            Images = new PharmacyImagesTabViewModel
            {
                Image1Url = pharmacy.Image1Url,
                Image2Url = pharmacy.Image2Url,
                Image3Url = pharmacy.Image3Url
            },
            Contacts = new PharmacyContactsTabViewModel
            {
                ProvinceName = provinceName ?? string.Empty,
                CityName = cityName ?? string.Empty,
                Address = pharmacy.Address,
                Phone = pharmacy.Phone,
                PharmacyManagerMobile = pharmacy.PharmacyManagerMobile,
                InventoryManagerMobile = pharmacy.InventoryManagerMobile,
                PharmacyTechnicianMobile = pharmacy.PharmacyTechnicianMobile
            },
            Insurances = new PharmacyInsuranceTabViewModel
            {
                BaseInsurances = providerOptions
                    .Where(item => item.InsuranceType == InsuranceProvider.BaseInsuranceType)
                    .Select(item => new PharmacySelectionOptionViewModel(
                        item.Id,
                        item.Name,
                        selectedProviderIds.Contains(item.Id)))
                    .ToArray(),
                SupplementaryInsurances = providerOptions
                    .Where(item => item.InsuranceType == InsuranceProvider.SupplementaryInsuranceType)
                    .Select(item => new PharmacySelectionOptionViewModel(
                        item.Id,
                        item.Name,
                        selectedProviderIds.Contains(item.Id)))
                    .ToArray()
            }
        };
    }

    public async Task<bool> SaveDistrictAsync(
        int pharmacyAccountId,
        int? districtId,
        CancellationToken cancellationToken = default)
    {
        var pharmacy = await GetOwnedPharmacyAsync(pharmacyAccountId, cancellationToken);
        if (pharmacy is null || !districtId.HasValue)
        {
            return false;
        }

        var district = await context.DarooyabCityDistricts
            .SingleOrDefaultAsync(
                item => item.Id == districtId.Value &&
                        item.IsActive &&
                        item.CityId == pharmacy.CityIdFk,
                cancellationToken);
        if (district is null)
        {
            return false;
        }

        pharmacy.DistrictId = district.Id;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<PharmacySpecialtiesSaveStatus> SaveSpecialtiesAsync(
        int pharmacyAccountId,
        IReadOnlyCollection<int>? selectedIds,
        CancellationToken cancellationToken = default)
    {
        var pharmacy = await GetOwnedPharmacyAsync(pharmacyAccountId, cancellationToken);
        if (pharmacy is null)
        {
            return PharmacySpecialtiesSaveStatus.InvalidSelection;
        }

        var ids = (selectedIds ?? [])
            .Where(id => id > 0)
            .Distinct()
            .ToArray();
        if (ids.Length > 2)
        {
            return PharmacySpecialtiesSaveStatus.InvalidCount;
        }

        var validIds = await context.PharmacySpecialties
            .AsNoTracking()
            .Where(item => ids.Contains(item.ID) && !item.IsDeleted && item.IsActive)
            .Select(item => item.ID)
            .ToArrayAsync(cancellationToken);
        if (validIds.Length != ids.Length)
        {
            return PharmacySpecialtiesSaveStatus.InvalidSelection;
        }

        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);
        var existingMaps = await context.PharmacySpecialtyMaps
            .Where(item => item.PharmacyId_fk == pharmacy.Id && !item.IsDeleted)
            .ToListAsync(cancellationToken);
        foreach (var map in existingMaps)
        {
            map.IsDeleted = true;
            map.IsActive = false;
        }

        foreach (var id in ids)
        {
            context.PharmacySpecialtyMaps.Add(new PharmacySpecialtyMap
            {
                PharmacyId_fk = pharmacy.Id,
                PharmacySpecialtyId_fk = id,
                IsDeleted = false,
                IsActive = true,
                CreateDate = DateTime.Now
            });
        }

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return PharmacySpecialtiesSaveStatus.Succeeded;
    }

    public async Task<bool> SaveContactsAsync(
        int pharmacyAccountId,
        PharmacyContactForm form,
        CancellationToken cancellationToken = default)
    {
        var pharmacy = await GetOwnedPharmacyAsync(pharmacyAccountId, cancellationToken);
        if (pharmacy is null)
        {
            return false;
        }

        pharmacy.PharmacyManagerMobile = form.PharmacyManagerMobile;
        pharmacy.InventoryManagerMobile = form.InventoryManagerMobile;
        pharmacy.PharmacyTechnicianMobile = form.PharmacyTechnicianMobile;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<PharmacyInsuranceSaveStatus> SaveInsurancesAsync(
        int pharmacyAccountId,
        IReadOnlyCollection<int>? selectedBaseIds,
        IReadOnlyCollection<int>? selectedSupplementaryIds,
        CancellationToken cancellationToken = default)
    {
        var pharmacy = await GetOwnedPharmacyAsync(pharmacyAccountId, cancellationToken);
        if (pharmacy is null)
        {
            return PharmacyInsuranceSaveStatus.InvalidSelection;
        }

        var baseIds = (selectedBaseIds ?? []).Where(id => id > 0).Distinct().ToArray();
        var supplementaryIds = (selectedSupplementaryIds ?? [])
            .Where(id => id > 0)
            .Distinct()
            .ToArray();
        if (baseIds.Length == 0)
        {
            return PharmacyInsuranceSaveStatus.BaseInsuranceRequired;
        }

        var submittedIds = baseIds.Concat(supplementaryIds).Distinct().ToArray();
        var validProviders = await context.InsuranceProviders
            .AsNoTracking()
            .Where(item => submittedIds.Contains(item.Id) && !item.IsDeleted && item.IsActive)
            .Select(item => new { item.Id, item.InsuranceType })
            .ToArrayAsync(cancellationToken);
        var validBaseIds = validProviders
            .Where(item => item.InsuranceType == InsuranceProvider.BaseInsuranceType)
            .Select(item => item.Id)
            .ToHashSet();
        var validSupplementaryIds = validProviders
            .Where(item => item.InsuranceType == InsuranceProvider.SupplementaryInsuranceType)
            .Select(item => item.Id)
            .ToHashSet();
        if (baseIds.Any(id => !validBaseIds.Contains(id)) ||
            supplementaryIds.Any(id => !validSupplementaryIds.Contains(id)))
        {
            return PharmacyInsuranceSaveStatus.InvalidSelection;
        }

        await using var transaction = await context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);
        var existingContracts = await context.PharmacyInsuranceContracts
            .Where(item => item.PharmacyIdFk == pharmacy.Id && !item.IsDeleted)
            .ToListAsync(cancellationToken);
        foreach (var contract in existingContracts)
        {
            contract.IsDeleted = true;
            contract.IsActive = false;
        }

        foreach (var providerId in baseIds.Concat(supplementaryIds).Distinct())
        {
            context.PharmacyInsuranceContracts.Add(new PharmacyInsuranceContract
            {
                PharmacyIdFk = pharmacy.Id,
                InsuranceProviderIdFk = providerId,
                IsDeleted = false,
                IsActive = true,
                CreateDate = DateTime.Now
            });
        }

        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return PharmacyInsuranceSaveStatus.Succeeded;
    }

    public async Task<bool> SaveLocationAsync(
        int pharmacyAccountId,
        decimal latitude,
        decimal longitude,
        CancellationToken cancellationToken = default)
    {
        var pharmacy = await GetOwnedPharmacyAsync(pharmacyAccountId, cancellationToken);
        if (pharmacy is null ||
            latitude is < -90 or > 90 ||
            longitude is < -180 or > 180)
        {
            return false;
        }

        pharmacy.Latitude = latitude;
        pharmacy.Longitude = longitude;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<string?[]> GetImageUrlsAsync(
        int pharmacyAccountId,
        CancellationToken cancellationToken = default)
    {
        var pharmacyId = await GetPharmacyIdAsync(pharmacyAccountId, cancellationToken);
        if (!pharmacyId.HasValue)
        {
            return [];
        }

        var imageUrls = await context.Pharmacies
            .AsNoTracking()
            .Where(item => item.Id == pharmacyId.Value)
            .Select(item => new { item.Image1Url, item.Image2Url, item.Image3Url })
            .SingleOrDefaultAsync(cancellationToken);
        return imageUrls is null
            ? []
            : [imageUrls.Image1Url, imageUrls.Image2Url, imageUrls.Image3Url];
    }

    public async Task<bool> SaveImageUrlsAsync(
        int pharmacyAccountId,
        IReadOnlyDictionary<int, string?> imageUrls,
        CancellationToken cancellationToken = default)
    {
        var pharmacy = await GetOwnedPharmacyAsync(pharmacyAccountId, cancellationToken);
        if (pharmacy is null)
        {
            return false;
        }

        foreach (var (imageNumber, imageUrl) in imageUrls)
        {
            switch (imageNumber)
            {
                case 1:
                    pharmacy.Image1Url = imageUrl;
                    break;
                case 2:
                    pharmacy.Image2Url = imageUrl;
                    break;
                case 3:
                    pharmacy.Image3Url = imageUrl;
                    break;
                default:
                    return false;
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private async Task<Pharmacy?> GetOwnedPharmacyAsync(
        int pharmacyAccountId,
        CancellationToken cancellationToken)
    {
        var pharmacyId = await GetPharmacyIdAsync(pharmacyAccountId, cancellationToken);
        return pharmacyId.HasValue
            ? await context.Pharmacies.SingleOrDefaultAsync(
                item => item.Id == pharmacyId.Value,
                cancellationToken)
            : null;
    }

    private Task<int?> GetPharmacyIdAsync(
        int pharmacyAccountId,
        CancellationToken cancellationToken) =>
        context.PharmacyAccounts
            .AsNoTracking()
            .Where(account => account.Id == pharmacyAccountId)
            .Select(account => (int?)account.PharmacyIdFk)
            .SingleOrDefaultAsync(cancellationToken);
}
