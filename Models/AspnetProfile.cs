using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class AspnetProfile
    {
        public Guid UserId { get; set; }
        public string PropertyNames { get; set; }
        public string PropertyValuesString { get; set; }
        public byte[] PropertyValuesBinary { get; set; }
        public DateTime LastUpdatedDate { get; set; }

        public virtual AspnetUser User { get; set; }
    }
}
