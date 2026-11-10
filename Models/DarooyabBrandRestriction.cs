
public enum BrandRestrictionTypeEnum  : byte {
  // ممنوعیت کامل اعلام موجودی
  FullyProhibited ,  
  // اعلام موجودی با هشدار به داروخانه (هشدار عمومی)
  WarningToPharmacy ,  
  // اعلام موجودی با هشدار محرمانه به مدیر سیستم (هشدار داخلی)
  ConfidentialWarningToAdmin 
}
public class DarooyabBrandRestriction
{
    public int Id { get; set; }
    public BrandRestrictionTypeEnum BrandRestrictionType { get; set; }
    public bool IsActive { get; set; } = true;
    public long IRC { get; set; }
    public string? Description { get; set; }
    public DateTime CreateDate { get; set; }
}