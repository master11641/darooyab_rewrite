using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class TblBrandAttribute
    {
        public int TblBrandAttributeId { get; set; }
        public int? DrugsNameIdFk { get; set; }
        public long TblBrandAttributeIrc { get; set; }
        public string? TblBrandAttributePageTitle { get; set; }
        public string? TblBrandAttributePageDescription { get; set; }
        public string? TblBrandAttributeContentEn { get; set; }
        public string? TblBrandAttributeContentFa { get; set; }
        public int? TblBrandAttributeRank { get; set; }
        public string? TblBrandAttributeShopUrl { get; set; }
        /// <summary>
        /// بدون نسخه است؟
        /// </summary>
        public bool? IsOtc { get; set; }
        /// <summary>
        /// شیمی درمانی است؟
        /// </summary>
        public bool? IsChemotherapy { get; set; }
        /// <summary>
        /// تحت نظارت است؟
        /// </summary>
        public bool? IsUnderSupervision { get; set; }
        /// <summary>
        /// دارو بیمارستانی است؟
        /// </summary>
        public bool? IsHospital { get; set; }
        /// <summary>
        /// کاربرد درمانی
        /// </summary>
        public string? TherapeuticApplication { get; set; }
        public int? StorageTemperatureId { get; set; }
        public string? Image1Url { get; set; }
        public string? Image2Url { get; set; }
        public string? Image3Url { get; set; }
        public string? VoiceUrl { get; set; }
        public string? ClipUrl { get; set; }
        public string? AdvertisingUrl { get; set; }
        public decimal? Price { get; set; }
        public byte? PriceUnitNum { get; set; }
        public int? LastPriceEditorPharmacyFk { get; set; }
        public DateTime? LastPriceEditDate { get; set; }

        public virtual DrugsName? DrugsNameIdFkNavigation { get; set; }
        public virtual Category? StorageTemperature { get; set; }
    }



}
