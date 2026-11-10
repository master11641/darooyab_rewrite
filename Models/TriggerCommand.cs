using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class TriggerCommand
    {
        public int Id { get; set; }
        public string SqlCommand { get; set; }
        public string TableName { get; set; }
        public string ActionType { get; set; }
        public DateTime? TimeStamp { get; set; }
        public int? HospitalId { get; set; }
    }
}
