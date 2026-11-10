using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class SlidInfoTopIrc
    {
        public decimal FileId { get; set; }
        public int? FilePageIdFk { get; set; }
        public int? FileGidFk { get; set; }
        public string FileName { get; set; }
        public string FileAlternative { get; set; }
        public string FileTooltip { get; set; }
        public string FilePath { get; set; }
        public int? FileSize { get; set; }
        public string FileLink { get; set; }
        public bool? FileVisible { get; set; }
        public DateTime? FileDate { get; set; }
    }
}
