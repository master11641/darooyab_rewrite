using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class DarooyabUniversityMS
    {
        public int Id { get; set; }
        public string Title { get; set; }        
        public int? ProvinceId { get; set; }
        [ForeignKey("ProvinceId")]
        public DarooyabProvince? DarooyabProvince { get; set; }
    }
}