using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class Section
    {
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public int? SectionParentId { get; set; }
        public int? SectionOrder { get; set; }
    }
}
