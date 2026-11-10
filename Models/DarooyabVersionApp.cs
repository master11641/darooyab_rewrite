
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{

    public enum DrooyabPlatform
    {
        Android,
        IOS,
        Web,
        Windows,
        Mac,

    }
    public class DarooyabVersionApp
    {
        public int Id { get; set; }
        public DrooyabPlatform DrooyabPlatform { get; set; }
        public int VersionNumber { get; set; }
        public string? VersionFullNumber { get; set; }
        public DateTime ReleaseDate { get; set; }
        public bool UpdateRequired { get; set; }
        public string? Description { get; set; }

    }
}