using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class PregnancyCategory
    {
        public PregnancyCategory()
        {
            DrugsGenericInfos = new HashSet<DrugsGenericInfo>();
        }

        public byte PregnancyCategoryId { get; set; }
        public string? PregnancyCategoryCode { get; set; }
        public string? PregnancyCategoryDes { get; set; }
        public string? PregnancyCategoryDesFa { get; set; }

        public virtual ICollection<DrugsGenericInfo> DrugsGenericInfos { get; set; }
    }
}
