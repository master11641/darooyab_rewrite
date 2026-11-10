using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class Toxication
    {
        public Toxication()
        {
            DrugsGenericInfos = new HashSet<DrugsGenericInfo>();
        }
        public int Id { get; set; }
        [UIHint("CKEditor")]
        [Display(Name = "موضوع")]

        public string ToxicationSubject { get; set; }
        [UIHint("CKEditor")]
        [Display(Name = "محتوی")]

        public string ToxicationContent { get; set; }
        public DateTime InsertDate { get; set; }
        public DateTime EditDate { get; set; }
        public virtual ICollection<DrugsGenericInfo> DrugsGenericInfos { get; set; }

    }
}