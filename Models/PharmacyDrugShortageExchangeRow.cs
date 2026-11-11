namespace darooyab.Models;

public sealed class PharmacyDrugShortageExchangeRow
{
    public int Id { get; set; }
    public int PharmacyAccountIdFk { get; set; }
    public string? DrugName { get; set; }
    public int? DrugCount { get; set; }
    public DateTime? DrugExpireDate { get; set; }
    public double? UnitPrice { get; set; }
    public string? DrugUnitNum { get; set; }
    public DateTime? RegistrationDateTime { get; set; }
    public string? ExcessPharmacyProvinceName { get; set; }
    public string? ExcessPharmacyTownshipName { get; set; }
    public string? ExcessPharmacyAddress { get; set; }
    public string? ExcessPharmacyTell { get; set; }
    public string? ExcessPharmacyName { get; set; }
    public string? ExcessPharmacyMobile { get; set; }
    public int? SumRequireCountExchAlready { get; set; }
    public string? ShortageThatMyExcessDrugName { get; set; }
}
