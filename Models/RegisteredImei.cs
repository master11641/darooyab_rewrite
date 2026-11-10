using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class RegisteredImei
    {
        public int Id { get; set; }
        public string Imei { get; set; }
        public int? UserId { get; set; }
    }
}
