using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class ViewDrugsDetailForApp
    {
        public int DrugId { get; set; }
        public short? GroupId { get; set; }
        public string BrandNameFa { get; set; }
        public string BrandNameEn { get; set; }
        public short? NewDrugRating { get; set; }
        public string GenericEn { get; set; }
        public string GenericFa { get; set; }
        public bool? IsOtc { get; set; }
        public bool? IsChemotherapy { get; set; }
        public bool? IsUnderSupervision { get; set; }
        public bool? IsHospital { get; set; }
        public int? StorageTemperatureId { get; set; }
        public string TherapeuticApplication { get; set; }
        public short? ImporterCompanyId { get; set; }
        public short? FactoryId { get; set; }
        public int? DistributorId { get; set; }
        public string ClipUrl { get; set; }
        public string SoundUrl { get; set; }
    }
}
