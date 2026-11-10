using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class DarooyabSuperSpecialty
    {
        public DarooyabSuperSpecialty()
        {
            DarooyabSpecialtySuperSpecialties = new HashSet<DarooyabSpecialtySuperSpecialty>();
            DarooyabSuperServices = new HashSet<DarooyabSuperService>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string? FullTitle { get; set; }
        public int OrderIndex { get; set; }
        public ICollection<DarooyabSpecialtySuperSpecialty> DarooyabSpecialtySuperSpecialties { get; set; }
        public ICollection<DarooyabSuperService> DarooyabSuperServices { get; set; }
    }
}