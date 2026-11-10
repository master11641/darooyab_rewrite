using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class LogHistory
    {
        public int Id { get; set; }
        public int AccountIdFk { get; set; }
        public int AccountTypeFk { get; set; }
        public int LogTypeIdFk { get; set; }
        public DateTime RegisterTime { get; set; }
        public int? PharmacyChangeIdFk { get; set; }
    }
}
