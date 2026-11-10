using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class SectionPerHospital
    {
        public int Id { get; set; }
        public int? HospitalId { get; set; }
        public int? SectionId { get; set; }
    }
}
