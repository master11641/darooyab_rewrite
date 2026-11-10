using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class TblNewsGroup
    {
        public TblNewsGroup()
        {
            TblNews = new HashSet<TblNews>();
        }

        public int TblNewsGroupId { get; set; }
        public string TblNewsGroupEngTitle { get; set; }
        public string TblNewsGroupPersTitle { get; set; }
        public string TblNewsGroupDescription { get; set; }

        public virtual ICollection<TblNews> TblNews { get; set; }
    }
}
