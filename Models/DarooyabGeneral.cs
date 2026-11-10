
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class DarooyabGeneralItem
    {
        public DarooyabGeneralItem()
        {
            DarooyabGeneralServices = new HashSet<DarooyabGeneralService>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public int OrderIndex { get; set; }
        public ICollection<DarooyabGeneralService> DarooyabGeneralServices { get; set; }
    }
}