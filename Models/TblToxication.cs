using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class TblToxication
    {
        public TblToxication()
        {
            GenericInfoAttributes = new HashSet<GenericInfoAttribute>();
            TblGenericPageAttributes = new HashSet<TblGenericPageAttribute>();
        }

        public int TblToxicationId { get; set; }
        public string TblToxicationSubject { get; set; }
        public string TblToxicationPageTitle { get; set; }
        public string TblToxicationPageDescription { get; set; }
        public int? TblToxicationRank { get; set; }
        public string TblToxicationContent { get; set; }
        public bool? TblToxicationIsSpecial { get; set; }
        public string TblToxicationImageUrl { get; set; }

        public virtual ICollection<GenericInfoAttribute> GenericInfoAttributes { get; set; }
        public virtual ICollection<TblGenericPageAttribute> TblGenericPageAttributes { get; set; }
    }
}
