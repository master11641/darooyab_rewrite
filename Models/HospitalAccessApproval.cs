using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class HospitalAccessApproval
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public int? HospitalId { get; set; }
        public string Status { get; set; }
        public DateTime? TimeStamp { get; set; }
        public string AffectedUser { get; set; }
        public bool? IsNew { get; set; }
    }
}
