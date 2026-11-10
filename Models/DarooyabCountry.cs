using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class DarooyabCountry
    {
        public DarooyabCountry()
        {
            DarooyabProvinces = new HashSet<DarooyabProvince>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderIndex { get; set; }
        public bool IsActive { get; set; }
        public ICollection<DarooyabProvince> DarooyabProvinces { get; set; }

    }
}