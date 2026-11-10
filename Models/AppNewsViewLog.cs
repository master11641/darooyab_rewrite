using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class AppNewsViewLog
    {
        public int Id { get; set; }
        public int? NewsIdFk { get; set; }
        public int? ActivityIdFk { get; set; }
        public int SpecialtyIdFk { get; set; }
        public int ProvinceIdFk { get; set; }
        public DateTime? DateTimeView { get; set; }
        public bool? IsFullNewsSeen { get; set; }

        public virtual Category ActivityIdFkNavigation { get; set; }
        public virtual DarooyabAppNews NewsIdFkNavigation { get; set; }
    }
}
