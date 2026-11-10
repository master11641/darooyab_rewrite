using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class User
    {
        public decimal Id { get; set; }
        public string Email { get; set; }
        public string Fname { get; set; }
        public string Lname { get; set; }
        public byte? Gender { get; set; }
        public string Mobile { get; set; }
        public byte? Madrak { get; set; }
        public string RegDate { get; set; }
    }
}
