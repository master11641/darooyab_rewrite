

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public enum DrooyabNotificationPlatform:byte
    {
        Application,
        Website
    }
    public class DarooyabDrugShortage
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public DateTime RegisterDate { get; set; }
        public DrooyabNotificationPlatform NotificationPlatform { get; set; }
        public int CityId { get; set; }
        public int DrugId { get; set; }
        public long? IRC { get; set; }
        public int? DosageFormId { get; set; }
        [ForeignKey("UserId")]
        public DarooyabUser? DarooyabUser { get; set; }
        [ForeignKey("CityId")]
        public DarooyabCity darooyabCity { get; set; }
        [ForeignKey("DosageFormId")]
        public DosageForm? dosageForm { get; set; }
        public bool IsVerified { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!IRC.HasValue && !DosageFormId.HasValue)
            {
                yield return new ValidationResult(
                    "حداقل یکی از فیلدهای IRC یا DosageFormId باید تکمیل شود.",
                    new[] { nameof(IRC), nameof(DosageFormId) }
                );
            }
        }
        
    }
}