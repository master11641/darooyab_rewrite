using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class ViewPharmacyDatum
    {
        public string FounderName { get; set; }
        public DateTime? RegistrationDateTime { get; set; }
        public int? PharmacyIdFk { get; set; }
        public string Name { get; set; }
        public string DrugName { get; set; }
    }
}
