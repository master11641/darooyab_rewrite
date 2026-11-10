using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class TempTaminDirect
    {
        public int Id { get; set; }
        public string FdoGenerationCode { get; set; }
        public string TaminDirectCode { get; set; }
        public string TaminDirectIsAccept { get; set; }
        public string TaminDirectLevel { get; set; }
        public string TaminDirectIsMama { get; set; }
        public string TaminDirectIsDentist { get; set; }
        public string TaminDirectIsRadiologist { get; set; }
        public string TaminDirectIsSite { get; set; }
        public string TaminDirectIsHospital { get; set; }
        public string TaminDirectIsChemotherapy { get; set; }
        public string TaminDirectIsErjaDakheli { get; set; }
    }
}
