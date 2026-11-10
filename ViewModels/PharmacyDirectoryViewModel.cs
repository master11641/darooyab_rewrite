namespace darooyab_rewrite.ViewModels;

public sealed record PharmacyFilterOption(int Id, string Name);

public sealed record PharmacyDirectoryItem(
    int Id,
    string Name,
    string City,
    string Province,
    string? Address,
    string? Phone);

public sealed record PharmacyDirectoryViewModel(
    PagedResult<PharmacyDirectoryItem> Pharmacies,
    int ProvinceId,
    int CityId,
    int DistrictId,
    int PharmacySpecialtyId,
    string PharmacyName,
    IReadOnlyList<PharmacyFilterOption> Provinces,
    IReadOnlyList<PharmacyFilterOption> Cities,
    IReadOnlyList<PharmacyFilterOption> Districts,
    IReadOnlyList<PharmacyFilterOption> Specialties,
    bool DataUnavailable = false);
