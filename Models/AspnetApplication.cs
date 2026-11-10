using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class AspnetApplication
    {
        public AspnetApplication()
        {
            AspnetMemberships = new HashSet<AspnetMembership>();
            AspnetPaths = new HashSet<AspnetPath>();
            AspnetRoles = new HashSet<AspnetRole>();
            AspnetUsers = new HashSet<AspnetUser>();
        }

        public string ApplicationName { get; set; }
        public string LoweredApplicationName { get; set; }
        public Guid ApplicationId { get; set; }
        public string Description { get; set; }

        public virtual ICollection<AspnetMembership> AspnetMemberships { get; set; }
        public virtual ICollection<AspnetPath> AspnetPaths { get; set; }
        public virtual ICollection<AspnetRole> AspnetRoles { get; set; }
        public virtual ICollection<AspnetUser> AspnetUsers { get; set; }
    }
}
