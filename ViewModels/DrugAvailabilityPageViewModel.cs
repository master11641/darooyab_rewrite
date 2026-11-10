namespace darooyab_rewrite.ViewModels;

public sealed record DrugAvailabilityItem(
    int PharmacyId,
    string PharmacyName,
    string Province,
    string City,
    string? Address,
    string? Phone,
    DateTime? RequestExpireDate,
    long Irc,
    string? PersianBrandName,
    string? EnglishBrandName,
    short? RankNumber);

public sealed record DrugAvailabilityPageViewModel(
    bool IsBrand,
    int DrugId,
    long Irc,
    string DrugName,
    string? GenericName,
    string? GenericUrl,
    string? BrandUrl,
    int ProvinceId,
    int CityId,
    int DistrictId,
    int PharmacySpecialtyId,
    bool ByDosageForm,
    IReadOnlyList<PharmacyFilterOption> Provinces,
    IReadOnlyList<PharmacyFilterOption> Cities,
    IReadOnlyList<PharmacyFilterOption> Districts,
    IReadOnlyList<PharmacyFilterOption> Specialties,
    PagedResult<DrugAvailabilityItem> Results,
    bool DataUnavailable = false);
