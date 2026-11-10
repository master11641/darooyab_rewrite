using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class AspnetUser
    {
        public AspnetUser()
        {
            AspnetPersonalizationPerUsers = new HashSet<AspnetPersonalizationPerUser>();
            Roles = new HashSet<AspnetRole>();
        }

        public Guid ApplicationId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string LoweredUserName { get; set; }
        public string MobileAlias { get; set; }
        public bool IsAnonymous { get; set; }
        public DateTime LastActivityDate { get; set; }

        public virtual AspnetApplication Application { get; set; }
        public virtual AspnetMembership AspnetMembership { get; set; }
        public virtual AspnetProfile AspnetProfile { get; set; }
        public virtual ICollection<AspnetPersonalizationPerUser> AspnetPersonalizationPerUsers { get; set; }

        public virtual ICollection<AspnetRole> Roles { get; set; }
    }
}
