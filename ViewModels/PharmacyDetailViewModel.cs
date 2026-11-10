namespace darooyab_rewrite.ViewModels;

public sealed record PharmacyAvailabilityItem(
    int RequestId,
    long Irc,
    string BrandName,
    string GenericName,
    DateTime ExpireDate,
    short RankNumber,
    string DetailUrl);

public sealed record OtherPharmacyReport(
    int PharmacyId,
    string Name,
    string City,
    string Province,
    string? Phone,
    DateTime ExpireDate,
    short RankNumber,
    string DetailUrl);

public sealed record PharmacyDetailViewModel(
    int PharmacyId,
    string Name,
    string City,
    string Province,
    string Address,
    string? Phone,
    string? SecondaryPhone,
    string? Introduction,
    decimal? Latitude,
    decimal? Longitude,
    IReadOnlyList<string> Images,
    IReadOnlyList<string> ProductImages,
    IReadOnlyList<string> Specialties,
    IReadOnlyList<PharmacyFilterOption> ReportingProvinces,
    IReadOnlyList<PharmacyFilterOption> ReportingCities,
    int ReportingProvinceId,
    int ReportingCityId,
    IReadOnlyList<OtherPharmacyReport> OtherPharmacies,
    PagedResult<PharmacyAvailabilityItem> Availability,
    long? Irc,
    string? BrandName,
    string? EnglishBrandName,
    string? GenericName,
    string? GenericUrl,
    string? TherapeuticApplication,
    DateTime? ProductExpireDate)
{
    public bool IsProductView => Irc.HasValue;
    public bool HasActiveProductAvailability => ProductExpireDate.HasValue;
}
