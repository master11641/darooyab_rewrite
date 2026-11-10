using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class VwAspnetApplication
    {
        public string ApplicationName { get; set; }
        public string LoweredApplicationName { get; set; }
        public Guid ApplicationId { get; set; }
        public string Description { get; set; }
    }
}
