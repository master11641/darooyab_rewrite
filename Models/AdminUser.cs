using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class AdminUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
    }
}
