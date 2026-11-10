using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class DarooyabSession
    {
        public int Id { get; set; }
        public string? FcmToken { get; set; }
        public string? Jwt { get; set; }
        public string? VersionNumber { get; set; }
        public string? DeviceName { get; set; }
        public DateTime LoginDate { get; set; }
        public bool IsActive { get; set; }
        public byte? Platform { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public DarooyabUser DarooyabUser { get; set; }

    }
}