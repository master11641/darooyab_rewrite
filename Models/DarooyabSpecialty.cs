using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class DarooyabSpecialty
    {
        public DarooyabSpecialty()
        {
            DarooyabSpecialtySuperSpecialties = new HashSet<DarooyabSpecialtySuperSpecialty>();
            DarooyabSpecialtyServices = new HashSet<DarooyabSpecialtyService>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string? FullTitle { get; set; }
        public int OrderIndex { get; set; }
        public int? ActivityId { get; set; }
        [ForeignKey("ActivityId")]
        public DarooyabActivity? DarooyabActivity { get; set; }
        public ICollection<DarooyabSpecialtySuperSpecialty> DarooyabSpecialtySuperSpecialties { get; set; }
        public ICollection<DarooyabSpecialtyService> DarooyabSpecialtyServices { get; set; }
    }
}