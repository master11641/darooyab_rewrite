using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class Notification
    {
        public int Id { get; set; }
        public DateTime? SendTime { get; set; }
        public int DarooyabAppNewsIdFk { get; set; }
        public bool? SendNotificationStatus { get; set; }
        public string? FirebaseResult { get; set; }
    }
}
