using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class TblAttachmentFileToGenericAttribute
    {
        public int TblAttachmentFileToGenericAttributeId { get; set; }
        public string TblAttachmentFileToGenericAttributeFileDirUrl { get; set; }
        public string TblAttachmentFileToGenericAttributeDescription { get; set; }
        public int TblGenericPageAttributeIdFk { get; set; }

        public virtual TblGenericPageAttribute TblGenericPageAttributeIdFkNavigation { get; set; }
    }
}
