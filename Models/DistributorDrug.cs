using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class DistributorDrug
    {
        public int Id { get; set; }
        public int? FactoryId { get; set; }
        public int? DrugId { get; set; }
        public long? Irc { get; set; }
        public string Exist { get; set; }
        public int? RecordId { get; set; }
        public string ClipUrl { get; set; }
        public string SoundUrl { get; set; }
        public short? NewDrugRating { get; set; }
    }
}
