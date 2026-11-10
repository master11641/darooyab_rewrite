
using System.Linq;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DNTPersianUtils.Core;
using darooyab_rewrite.Utilities;
namespace darooyab.Models
{
    public enum ServiceType
    {
        [Display(Name = "ویزیت علمی")]
        ScientificVisit,
        [Display(Name = "ویزیت علمی- ارسال نمونه رایگان")]
        SendFreeSample,
        [Display(Name = "کارآزمایی بالینی")]
        ClinicalTrial,
        [Display(Name = "دعوت به میهمانی")]
        PartyInvitation,
        [Display(Name = "تبلیغات")]
        Advertising,
        [Display(Name = "کد تخفیف")]
        DiscountCode,
        [Display(Name = "ثبت نام در وبینار")]
        RegisterInWebinar

    }
    public enum AuthenticateType
    {
        [Display(Name = "فاقد احراز هویت")]
        UnAuthenticated,
        [Display(Name = "احراز هویت با سرویس نظام پزشکی")]

        IRIMCAuthenticated,
        [Display(Name = "احراز هویت با عکس/پروانه")]

        PhotoAuthenticated

    }
    public class DarooyabService
    {
        public DarooyabService()
        {
            DarooyabSpecialtyServices = new HashSet<DarooyabSpecialtyService>();
            DarooyabSuperServices = new HashSet<DarooyabSuperService>();
            DarooyabGeneralServices = new HashSet<DarooyabGeneralService>();
            DarooyabProvinceServices = new HashSet<DarooyabProvinceService>();
            DarooyabCityServices = new HashSet<DarooyabCityService>();
            DarooyabServiceRegisters = new HashSet<DarooyabServiceRegister>();
            PharmacyLocations = new HashSet<DarooyabPharmacyLocation>();
        }
        public long Id { get; set; }
        [Display(Name = "نوع سرویس")]
        public ServiceType ServiceType { get; set; }
        [Display(Name = "وضعیت سرویس")]
        [UIHint("IsActive")]
        public bool IsActive { get; set; }
        [Display(Name = "ارسال پیامک پس از ثبت نام")]
        [UIHint("IsActiveNullable")]
        public bool? IsSendSms { get; set; }
        [UIHint("Multiline")]
        [Display(Name = "پیامک پس از ثبت نام")]
        public string? SmsMessage { get; set; }

        [Display(Name = "عنوان سرویس")]
        public string Title { get; set; }
        [Display(Name = "زیرعنوان سرویس")]
        public string SubTitle { get; set; }
        [Display(Name = "توضیحات سرویس")]
        [UIHint("Editor")]
        public string Description { get; set; }
        [Display(Name = "توضیحات پس از ثبت سرویس")]
        [UIHint("Multiline")]
        public string DescriptionAfterRegister { get; set; }
        [Display(Name = "شرکت مرتبط")]
        public short? FactoryId { get; set; }
        [Display(Name = "شرکت مرتبط")]
        [ForeignKey("FactoryId")]
        public virtual Factory? Factory { get; set; }
        [Display(Name = "تصویر")]
        [UIHint("ImageUpload")]
        public string? ImageUrl { get; set; }
        [Display(Name = "کلیپ")]
        [UIHint("Upload")]
        public string? ClipUrl { get; set; }
        [Display(Name = "کاتالوگ")]
        [UIHint("Upload")]
        public string? CatalogUrl { get; set; }
        [Display(Name = "تاریخ شروع")]
        public DateTime StartTime { get; set; } = DateTime.Now;
        [Display(Name = "تاریخ شروع")]
        [NotMapped]
        public string PersianStartTime
        {
            get
            {
                return PersianDateFormatter.Format(this.StartTime);
            }
            set
            {
                this.StartTime = (DateTime)value.ToGregorianDateTime();
            }
        }
        [Display(Name = "تاریخ پایان")]
        [NotMapped]
        public string PersianEndTime
        {
            get
            {
                return PersianDateFormatter.Format(this.EndTime);
            }
            set
            {
                this.EndTime = (DateTime)value.ToGregorianDateTime();
            }
        }
        [Display(Name = "تاریخ پایان")]
        public DateTime EndTime { get; set; } = DateTime.Now;
        [Display(Name = "حداکثر مخاطب")]
        public int MaximumAudience { get; set; }
        [Display(Name = "دریافت آدرس پستی")]
        [UIHint("Boolean")]
        public bool? PostalCodeRequired { get; set; }
        [Display(Name = "اجبار نمودن پاسخ به سوالات")]
        [UIHint("Boolean")]
        public bool? ResponseRequired { get; set; }
        [Display(Name = "اجبار نمودن تعیین داروخانه اطراف")]
        [UIHint("Boolean")]
        public bool? PharmacyRequired { get; set; }
        [Display(Name = "نوع احراز هویت")]
        public string? AuthenticateTypes { get; set; }
        // public AuthenticateType AuthenticateType { get; set; }
        [Display(Name = "عنوان کلید ثبت درخواست")]
        public string? RequestButtonTitle { get; set; }
        [Display(Name = "موسس داروخانه انسانی")]
        public bool? IsHumanPharmacyFounder { get; set; }
        [Display(Name = "عدم نمایش پزشکان عمومی ثبت نام کرده در سرویس ها")]
        public bool? GPRegistered { get; set; }
        [Display(Name = "نام فرستنده")]
        public string? SenderName { get; set; }
        [Display(Name = "آدرس فرستنده")]
        public string? SenderAddress { get; set; }
        [Display(Name = "کدپستی فرستنده")]
        public string? SenderZipCode { get; set; }
        [Display(Name = "تلفن فرستنده")]
        public string? SenderPhone { get; set; }
        [Display(Name = "تصویر لوگوی فرستنده")]
        [UIHint("ImageUpload")]
        public string? SenderLogoImageUrl { get; set; }

        [Display(Name = "فعالیت")]
        public int ActivityId { get; set; }

        [ForeignKey("ActivityId")]
        public DarooyabActivity? DarooyabActivity { get; set; }
        public ICollection<DarooyabSpecialtyService> DarooyabSpecialtyServices { get; set; }
        public ICollection<DarooyabSuperService> DarooyabSuperServices { get; set; }
        public ICollection<DarooyabGeneralService> DarooyabGeneralServices { get; set; }
        public ICollection<DarooyabProvinceService> DarooyabProvinceServices { get; set; }
        public ICollection<DarooyabCityService> DarooyabCityServices { get; set; }
        public ICollection<DarooyabServiceRegister> DarooyabServiceRegisters { get; set; }
        public ICollection<DarooyabPharmacyLocation> PharmacyLocations { get; set; }

    }
}