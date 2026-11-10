using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class DosageFormAttribute
    {
        public int DosageFormId { get; set; }
        public string? PatientAdvice { get; set; }
        public DateTime? PatientAdviceAddDate { get; set; }
        public string? PatientAdviceAddUser { get; set; }
        public DateTime? PatientAdviceEditDate { get; set; }
        public string? PatientAdviceEditUser { get; set; }
        public bool? PatientAdviceIsAccept { get; set; }

        public virtual DosageForm DosageForm { get; set; }
    }
}
