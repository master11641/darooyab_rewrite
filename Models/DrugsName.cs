using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace darooyab.Models
{
    public partial class DrugsName
    {
        public DrugsName()
        {
            TblBrandAttributes = new HashSet<TblBrandAttribute>();
            DarooyabDrugDistributions = new HashSet<DarooyabDrugDistribution>();
        }

        public int Id { get; set; }
        [Display(Name = "شناسه دارو")]
        public int DrugId { get; set; }
        [Display(Name = "نام دارو")]
        public string? DrugName { get; set; }
        [Required(ErrorMessage = "تکمیل نام فارسی دارو الزامی است")]
        [Display(Name = "نام فارسی برند")]
        [NotMapped]
        public string DrugNameFa { get; set; }
        [Required(ErrorMessage = "تکمیل نام انگلیسی دارو الزامی است")]
        [NotMapped]
        [Display(Name = "نام انگلیسی برند")]
        public string DrugNameEn { get; set; }
        [NotMapped]
        [UIHint("Multiline")]
        [Display(Name = "کاربرد درمانی")]
        public string? TherapeuticApplication { get; set; }
        [NotMapped]
        [Display(Name = "تصویر اول")]
        [UIHint("ImageUpload")]
        public string? Image1Url { get; set; }
        [NotMapped]
        [Display(Name = "تصویر دوم")]
        [UIHint("ImageUpload")]
        public string? Image2Url { get; set; }
        [NotMapped]
        [Display(Name = "تصویر سوم")]
        [UIHint("ImageUpload")]
        public string? Image3Url { get; set; }
        [NotMapped]
        [Display(Name = "عنوان صفحه")]
        [UIHint("Multiline")]
        public string? PageTitle { get; set; }
        [NotMapped]
        [Display(Name = "توضیحات صفحه")]
        [UIHint("Multiline")]
        public string? PageDescription { get; set; }
        public string DrugNameType { get; set; }
        public bool? DrugNameLanguage { get; set; }
        public int? AddDate { get; set; }
        public long? Irc { get; set; }
        [Display(Name = "کد نسخه نویسی")]
        public int? ErxCode { get; set; }
        public short? IrgenericId { get; set; }
        [Display(Name = "وارد کننده")]
        public short? ImporterCompanyId { get; set; }
        [Display(Name = "تولید کننده")]
        public short? FactoryId { get; set; }
        [Display(Name = "کشور تولید کننده")]
        public byte? ProducingCountryId { get; set; }
        public short? FactoryId2 { get; set; }
        public byte? ProducingCountryId2 { get; set; }
        public int? Rate { get; set; }
        [Display(Name = "برند جدید است؟")]
        public bool? IsNewestDrug { get; set; }
        public string? ATCcode { get; set; }
        public string? Keyword { get; set; }      
        public virtual Factory? Factory { get; set; }
        public virtual Factory? FactoryId2Navigation { get; set; }
        public virtual Factory? ImporterCompany { get; set; }
        public virtual Country? ProducingCountry { get; set; }
        public virtual Country? ProducingCountryId2Navigation { get; set; }
        public virtual ICollection<TblBrandAttribute> TblBrandAttributes { get; set; }
        public virtual ICollection<DarooyabDrugDistribution> DarooyabDrugDistributions { get; set; }
    }
}
