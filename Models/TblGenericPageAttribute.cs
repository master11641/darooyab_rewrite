using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class TblGenericPageAttribute
    {
        public TblGenericPageAttribute()
        {
            TblAttachmentFileToGenericAttributes = new HashSet<TblAttachmentFileToGenericAttribute>();
        }

        public int TblGenericPageAttributeId { get; set; }
        public int TblGenericPageAttributeDrugIdFk { get; set; }
        public string TblGenericPageAttributePageTitle { get; set; }
        public string TblGenericPageAttributePageDescription { get; set; }
        public string TblGenericPageAttributeGeneralInformation { get; set; }
        public int? ToxicationIdFk { get; set; }

        public virtual TblToxication ToxicationIdFkNavigation { get; set; }
        public virtual ICollection<TblAttachmentFileToGenericAttribute> TblAttachmentFileToGenericAttributes { get; set; }
    }
}
