using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class SpecificInformation
    {
        public int Id { get; set; }
        public decimal? Irc { get; set; }
        public string MedicalInformation { get; set; }
        public string PatientInformation { get; set; }
        public string SpecialFeature { get; set; }
        public string Keywords { get; set; }
        public bool? IsEnable { get; set; }
        public decimal? ConsumerPrices { get; set; }
        public DateTime? EntryDate { get; set; }
    }
}
