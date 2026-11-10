using System.ComponentModel.DataAnnotations.Schema;
using DNTPersianUtils.Core;

public class DarooyabServiceVM
{
    public Int64 Id { get; set; }
    public int ActivityId { get; set; }
    public string? AuthenticateTypes { get; set; }
    public string? CatalogUrl { get; set; }
    public string? ClipUrl { get; set; }
    public string Description { get; set; }
    public string DescriptionAfterRegister { get; set; }
    public DateTime? EndTime { get; set; }
    public Int16 FactoryId { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; }
    public bool? IsHumanPharmacyFounder { get; set; }
    public int MaximumAudience { get; set; }
    public bool PharmacyRequired { get; set; }
    public bool PostalCodeRequired { get; set; }
    public string? RequestButtonTitle { get; set; }
    public bool ResponseRequired { get; set; }
    public int ServiceType { get; set; }
    public DateTime? StartTime { get; set; }
    public string? SubTitle { get; set; }
    public string? Title { get; set; }
    public string? FactoryName { get; set; }
    [NotMapped]
    public string PersianEndTime
    {
        get
        {
            return darooyab_rewrite.Utilities.PersianDateFormatter.Format(this.EndTime);
        }
        set
        {
            this.EndTime = (DateTime)value.ToGregorianDateTime();
        }
    }
    [NotMapped]
    public string PersianStartTime
    {
        get
        {
            return darooyab_rewrite.Utilities.PersianDateFormatter.Format(this.StartTime);
        }
        set
        {
            this.StartTime = (DateTime)value.ToGregorianDateTime();
        }
    }
}
