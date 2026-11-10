using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class DrugsGenericInfo
    {
        public int? DrugId { get; set; }
        public string? LinkId { get; set; }
        public short? GroupId { get; set; }
        public string? SubGroupId { get; set; }
        /// <summary>
        /// برسی شود نوع داتده
        /// </summary>
        [Display(Name = "بارداری")]
        [UIHint("CKEditor")]
        public string? Bardary { get; set; }
        [Display(Name = "موارد مصرف")]
        [UIHint("CKEditor")]
        public string? MavaredMasraf { get; set; }
        [Display(Name = "مکانیزم")]
        [UIHint("CKEditor")]
        public string? Mekanism { get; set; }
        /// <summary>
        ///   D,Tفارما=برای گروه
        ///  H اجزا فراورده=برای گروه
        /// </summary>
        [UIHint("CKEditor")]
        public string? FarmaAjza { get; set; }
        [Display(Name = "منع مصرف")]
        [UIHint("CKEditor")]
        public string? ManeMasraf { get; set; }
        [Display(Name = "هشدار")]
        [UIHint("CKEditor")]
        public string? Hoshdar { get; set; }
        [Display(Name = "عوارض")]
        [UIHint("CKEditor")]
        public string? Avarez { get; set; }
        [Display(Name = "تداخل")]
        [UIHint("CKEditor")]
        public string? Tadakhol { get; set; }
        [Display(Name = "نکات")]
        [UIHint("CKEditor")]
        public string? Nokat { get; set; }
        [Display(Name = "اشکال")]
        [UIHint("CKEditor")]
        public string? Ashkal { get; set; }
        [Display(Name = "مقدار مصرف")]
        [UIHint("CKEditor")]
        public string? MeghdarMasraf { get; set; }
        /// <summary>
        ///  H مواد موثر= برای داروهای گیاهی گروه
        ///  T پروفایل=برای داروهای ترجمه گروه
        /// </summary>
        [UIHint("CKEditor")]
        public string? MavadMoaserProfile { get; set; }
        [Display(Name = "گروه بارداری مرتبط")]

        public byte? PregnancyCategoryId { get; set; }
        [Display(Name = "بارداری")]
        [UIHint("CKEditor")]
        public string? Pregnancy { get; set; }
        [Display(Name = "دوز بزرگسالان")]
        [UIHint("CKEditor")]
        public string? AdultDosing { get; set; }
        [Display(Name = "دوز اطفال")]
        [UIHint("CKEditor")]
        public string? PediatricDosing { get; set; }
        [Display(Name = "دوز سالمندان")]
        [UIHint("CKEditor")]
        public string? GeriatricDosing { get; set; }
        [Display(Name = "انتخاب کلیپ")]
        [UIHint("ImageUpload")]
        public string? ClipUrl { get; set; }
        [Display(Name = "تصویر اول")]
        [UIHint("ImageUpload")]
        public string? Image1Url { get; set; }
        [Display(Name = "تصویر دوم")]
        [UIHint("ImageUpload")]
        public string? Image2Url { get; set; }
        [Display(Name = "تصویر سوم")]
        [UIHint("ImageUpload")]
        public string? Image3Url { get; set; }
        [Display(Name = "فایل ضبط صدا")]
        public string? VoiceUrl { get; set; }
        public string? DrugsComURL { get; set; }
        public string? MedscapeComURL { get; set; }
        [UIHint("CKEditor")]
        [Display(Name = "اطلاعات عمومی")]
        public string? GeneralInformation { get; set; }
        public int? AhfsId { get; set; }
        public string? PageDescription { get; set; }
        public string? PageTitle { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }

        [Display(Name = "انتخاب مسمومیت")]
        public int? ToxicationIDFK { get; set; }
        public virtual Toxication? Toxication { get; set; }
        public virtual PregnancyCategory? PregnancyCategory { get; set; }
    }
}
