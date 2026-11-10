using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class AspnetRole
    {
        public AspnetRole()
        {
            Users = new HashSet<AspnetUser>();
        }

        public Guid ApplicationId { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string LoweredRoleName { get; set; }
        public string Description { get; set; }

        public virtual AspnetApplication Application { get; set; }

        public virtual ICollection<AspnetUser> Users { get; set; }
    }
}
