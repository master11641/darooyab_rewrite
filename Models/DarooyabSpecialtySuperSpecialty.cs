using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class DarooyabSpecialtySuperSpecialty
    {
        public int SpecialtyId { get; set; }
        public DarooyabSpecialty DarooyabSpecialty { get; set; }
        public int SuperSpecialtyId { get; set; }
        public DarooyabSuperSpecialty DarooyabSuperSpecialty { get; set; }
        public int OrderIndex { get; set; }

    }
}