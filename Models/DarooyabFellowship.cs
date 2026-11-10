
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class DarooyabFellowship
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int OrderIndex { get; set; }
        public string? FullTitle { get; set; }
        
    }
}