using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class FactoryType
    {
        public FactoryType()
        {
            Factories = new HashSet<Factory>();
        }

        public byte FactoryTypeId { get; set; }
        public string FactoryTypeName { get; set; }

        public virtual ICollection<Factory> Factories { get; set; }
    }
}
