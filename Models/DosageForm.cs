using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class DosageForm
    {
        public DosageForm()
        {
            PharmacyRequests = new HashSet<PharmacyRequest>();
            DarooyabDrugShortages = new HashSet<DarooyabDrugShortage>();
        }

        public int DosageFormId { get; set; }
        public string? DosageFormNameFa { get; set; }
        public string? DosageFormName { get; set; }
        public string? Form { get; set; }
        public int? FdoGenerationCode { get; set; }
        public int? DrugId { get; set; }
        public string? Emergency { get; set; }
        public int? KhadamatPrice1 { get; set; }
        public int? KhadamatPrice2 { get; set; }
        public int? KhadamatPrice3 { get; set; }
        public byte? AutomaticStopOrderDay { get; set; }
        public bool? IsSimilar { get; set; }
        public byte? TemperatureType { get; set; }
        public string? Antidote { get; set; }
        public bool? HighAlert { get; set; }
        public string? TaminDirectCode { get; set; }
        public bool? TaminDirectIsAccept { get; set; }
        public string? TaminDirectLevel { get; set; }
        public bool? TaminDirectIsMama { get; set; }
        public bool? TaminDirectIsDentist { get; set; }
        public bool? TaminDirectIsRadiologist { get; set; }
        public bool? TaminDirectIsSite { get; set; }
        public bool? TaminDirectIsHospital { get; set; }
        public bool? TaminDirectIsChemotherapy { get; set; }
        public bool? TaminDirectIsErjaDakheli { get; set; }
        public bool? TopWarningLabel { get; set; }

        public virtual DosageFormAttribute? DosageFormAttribute { get; set; }
        public virtual ICollection<PharmacyRequest> PharmacyRequests { get; set; }
        public virtual ICollection<DarooyabDrugShortage> DarooyabDrugShortages { get; set; }
    }
}
