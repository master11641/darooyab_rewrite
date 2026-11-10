using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class DarooyabProvince
    {
        public DarooyabProvince()
        {
            DarooyabCities = new HashSet<DarooyabCity>();
            DarooyabUniversityMs = new HashSet<DarooyabUniversityMS>();
            DarooyabProvinceServices = new HashSet<DarooyabProvinceService>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderIndex { get; set; }
        public bool IsActive { get; set; }
        public ICollection<DarooyabCity> DarooyabCities { get; set; }
        public int CountryId { get; set; }
        [ForeignKey("CountryId")]
        public DarooyabCountry DarooyabCountry { get; set; }
        public ICollection<DarooyabUniversityMS> DarooyabUniversityMs { get; set; }
        public ICollection<DarooyabProvinceService> DarooyabProvinceServices { get; set; }
    }
}