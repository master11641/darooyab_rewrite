using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class HospitalDosageForm
    {
        public int Id { get; set; }
        public int HospitalId { get; set; }
        public int DrugId { get; set; }
        public int DosageFormId { get; set; }
        public long Irc { get; set; }
        public string UserName { get; set; }
        public DateTime? LastChangeDate { get; set; }
    }
}
