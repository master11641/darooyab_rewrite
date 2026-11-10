using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class VwAspnetRole
    {
        public Guid ApplicationId { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string LoweredRoleName { get; set; }
        public string Description { get; set; }
    }
}
