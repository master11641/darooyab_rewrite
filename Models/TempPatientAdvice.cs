using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class TempPatientAdvice
    {
        public string DosageFormName { get; set; }
        public string DosageFormNameFa { get; set; }
        public int? FdoGenerationCode { get; set; }
        public string PatientAdvice { get; set; }
        public DateTime? PatientAdviceAddDate { get; set; }
    }
}
