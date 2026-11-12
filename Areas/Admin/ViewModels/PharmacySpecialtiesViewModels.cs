namespace darooyab_rewrite.Areas.Admin.ViewModels;

public sealed class PharmacySpecialtiesPageViewModel
{
    public required string PharmacyName { get; init; }
    public required PharmacyDistrictTabViewModel District { get; init; }
    public required PharmacySpecialtyTabViewModel Specialties { get; init; }
    public required PharmacyLocationTabViewModel Location { get; init; }
    public required PharmacyImagesTabViewModel Images { get; init; }
    public required PharmacyContactsTabViewModel Contacts { get; init; }
    public required PharmacyInsuranceTabViewModel Insurances { get; init; }
    public int ActiveTab { get; init; }
    public bool ShowDistrictTab { get; init; }
}

public sealed class PharmacyDistrictTabViewModel
{
    public int? DistrictId { get; init; }
    public required IReadOnlyList<PharmacySelectionOptionViewModel> Options { get; init; }
}

public sealed class PharmacySpecialtyTabViewModel
{
    public required IReadOnlyList<PharmacySelectionOptionViewModel> Options { get; init; }
    public int[] SelectedIds { get; init; } = [];
}

public sealed class PharmacyLocationTabViewModel
{
    public decimal? Latitude { get; init; }
    public decimal? Longitude { get; init; }
}

public sealed class PharmacyImagesTabViewModel
{
    public string? Image1Url { get; init; }
    public string? Image2Url { get; init; }
    public string? Image3Url { get; init; }
}

public sealed class PharmacyContactsTabViewModel
{
    public required string ProvinceName { get; init; }
    public required string CityName { get; init; }
    public string? Address { get; init; }
    public string? Phone { get; init; }
    public string? PharmacyManagerMobile { get; init; }
    public string? InventoryManagerMobile { get; init; }
    public string? PharmacyTechnicianMobile { get; init; }
}

public sealed class PharmacyInsuranceTabViewModel
{
    public required IReadOnlyList<PharmacySelectionOptionViewModel> BaseInsurances { get; init; }
    public required IReadOnlyList<PharmacySelectionOptionViewModel> SupplementaryInsurances { get; init; }
}

public sealed record PharmacySelectionOptionViewModel(int Id, string Name, bool Selected);

public sealed class PharmacyContactForm
{
    public string? PharmacyManagerMobile { get; set; }
    public string? InventoryManagerMobile { get; set; }
    public string? PharmacyTechnicianMobile { get; set; }
}

public sealed class PharmacyInsuranceForm
{
    public int[]? SelectedBaseInsuranceIds { get; set; }
    public int[]? SelectedSupplementaryInsuranceIds { get; set; }
}

public enum PharmacySpecialtiesSaveStatus
{
    Succeeded,
    InvalidCount,
    InvalidSelection
}

public enum PharmacyInsuranceSaveStatus
{
    Succeeded,
    BaseInsuranceRequired,
    InvalidSelection
}
