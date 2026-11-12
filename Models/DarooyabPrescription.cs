using DNTPersianUtils.Core;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public enum InsuranceType
    {
        [Display(Name = "تامین اجتماعی")]
        Ejtemaei,
        [Display(Name = "خدمات درمان")]
        Darman,
        [Display(Name = "ارتش")]
        Artesh,
        [Display(Name = "آزاد")]
        Azad


    }
    public class DarooyabPrescription
    {
        public DarooyabPrescription()
        {
            DarooyabPrescriptionSMSMessages = new HashSet<DarooyabPrescriptionSMSMessage>();
        }
        public int Id { get; set; }
        public int PharmacyId { get; set; }
        [ForeignKey("PharmacyId")]
        public Pharmacy Pharmacy { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string RegistrationUserName { get; set; }
        public string MobileNumber { get; set; }
        public int? ProvinceId { get; set; }
        [ForeignKey("ProvinceId")]
        public DarooyabProvince DarooyabProvince { get; set; }
        public int? CityId { get; set; }
        [ForeignKey("CityId")]
        public DarooyabCity darooyabCity { get; set; }
        public string? UploadImageUrl { get; set; }
        public long IRC { get; set; }
        public string? Description { get; set; }
        public InsuranceType InsuranceType { get; set; }
        public string? NationalCode { get; set; }
        public string? TrackingCode { get; set; }
        public string? VerificationCode { get; set; }
        public DateTime? VerificationCodeSentAt { get; set; }
        public bool? IsPhoneNumberVerified { get; set; }
        public int? UserId { get; set; }
        public bool? IsReviewed { get; set; }
        [ForeignKey("UserId")]
        public DarooyabUser DarooyabUser { get; set; }
        public ICollection<DarooyabPrescriptionSMSMessage> DarooyabPrescriptionSMSMessages { get; set; }

    }
}