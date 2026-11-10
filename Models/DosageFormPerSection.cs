using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class DosageFormPerSection
    {
        public int Id { get; set; }
        public int? SectionId { get; set; }
        public int? DosageFormId { get; set; }
    }
}
