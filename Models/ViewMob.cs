using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class ViewMob
    {
        public string DrugName { get; set; }
        public string Expr1 { get; set; }
        public string Expr4 { get; set; }
        public int DrugId { get; set; }
        public long? Irc { get; set; }
        public bool? IsOtc { get; set; }
        public bool? IsChemotherapy { get; set; }
        public bool? IsUnderSupervision { get; set; }
        public bool? IsHospital { get; set; }
        public string TherapeuticApplication { get; set; }
        public string StorageTemperature { get; set; }
        public string GroupNameFa { get; set; }
        public string Expr2 { get; set; }
        public string DrugNameType { get; set; }
        public bool? DrugNameLanguage { get; set; }
    }
}
