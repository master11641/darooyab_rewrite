using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class Group1
    {
        public Group1()
        {
            Diseases = new HashSet<Disease>();
        }

        public string? GroupName { get; set; }
        public string? GroupNameFa { get; set; }
        public byte? SubGroupCount { get; set; }
        public byte GroupId { get; set; }

        public virtual ICollection<Disease> Diseases { get; set; }
    }
}
