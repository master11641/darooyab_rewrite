namespace darooyab_rewrite.ViewModels;

public sealed record DoctorDirectoryAddress(
    string Type,
    string? Province,
    string? City,
    string Address,
    string? PostalCode,
    string? Phone1,
    string? Phone2,
    string? Website,
    decimal? Latitude = null,
    decimal? Longitude = null)
{
    public bool HasCoordinates =>
        Latitude is >= -90 and <= 90 &&
        Longitude is >= -180 and <= 180 &&
        (Latitude != 0 || Longitude != 0);
}

public sealed record DoctorDirectoryItem(
    int Id,
    string Name,
    string Family,
    string? ImageUrl,
    string? SystemNumber,
    int? ActivityId,
    string? ActivityTitle,
    string? Specialty,
    string? SubSpecialty,
    string? Fellowship,
    IReadOnlyList<DoctorDirectoryAddress> Addresses);

public sealed record DoctorDirectoryViewModel(
    PagedResult<DoctorDirectoryItem> Doctors,
    bool DataUnavailable = false);

public sealed record DoctorProfileViewModel(
    int Id,
    string Name,
    string Family,
    string? ImageUrl,
    int? ActivityId,
    string? ActivityTitle,
    string? Specialty,
    string? SubSpecialty,
    string? Fellowship,
    string? SystemNumber,
    string? City,
    string? Province,
    IReadOnlyList<DoctorDirectoryAddress> Addresses,
    IReadOnlyList<DrugComment>? Comments = null,
    int? LegacyAccountId = null);
