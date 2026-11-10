using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class DarooyabActivity
    {
        public DarooyabActivity()
        {
            DarooyabServices = new HashSet<DarooyabService>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public bool? SpecialtyRequired { get; set; }
        public int OrderIndex { get; set; }
        public int? SpecialtyId { get; set; }
        public string? PrefixCode { get; set; }
        public bool? IsAuthenticationByIRIMC { get; set; }

        [ForeignKey("SpecialtyId")]
        public DarooyabSpecialty? DarooyabSpecialty { get; set; }
        public virtual ICollection<DarooyabService> DarooyabServices { get; set; }
    }
}