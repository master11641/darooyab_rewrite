using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class DistributorsAgency
    {
        public int Id { get; set; }
        public int? FactoryId { get; set; }
        public int? ProvinceId { get; set; }
        public string Tel { get; set; }
        public string Description { get; set; }
    }
}
