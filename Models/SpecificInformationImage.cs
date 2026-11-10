using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class SpecificInformationImage
    {
        public int ImageId { get; set; }
        public int? SiIdFk { get; set; }
        public string SiImagePath { get; set; }
        public byte? SiImageType { get; set; }
        public DateTime? SiEntryDate { get; set; }
        public bool? SiIsEnable { get; set; }
    }
}
