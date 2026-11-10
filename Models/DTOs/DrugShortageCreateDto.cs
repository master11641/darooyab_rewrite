using System.ComponentModel.DataAnnotations;

namespace darooyab.Models.DTOs
{
  


    public class DrugShortageCreateDto
    {             
        public DrooyabNotificationPlatform NotificationPlatform { get; set; }
        public int CityId { get; set; }
        public int DrugId { get; set; }
        public long? IRC { get; set; }
        public int? DosageFormId { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!IRC.HasValue && !DosageFormId.HasValue)
            {
                yield return new ValidationResult(
                    "حداقل یکی از فیلدهای IRC یا DosageFormId باید مقدار داشته باشد.",
                    new[] { nameof(IRC), nameof(DosageFormId) }
                );
            }
        }
    }


    public class ShortageGroupItem
{
    public string Name { get; set; }
    public int Count { get; set; }
}

public class ShortageReportVM
{
    public List<ShortageGroupItem> DosageGroups { get; set; }
    public List<ShortageGroupItem> BrandGroups { get; set; }
    public List<DarooyabProvince> Provinces { get; set; }
}

}