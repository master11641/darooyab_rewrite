using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class DarooyabHistory
    {
        public long Id { get; set; }
        public int? DrugId { get; set; }
        public string? ActionType { get; set; }
        public int? UserId { get; set; }
        public DateTime CreateDate { get; set; }
       // public DarooyabUser? DarooyabUser { get; set; }
        public Int16 LogSourceType { get; set; }//0 Application  1 Website
        public int? BrandId { get; set; }
        public int? NewsId { get; set; }
        public string? Page { get; set; }
        public string? IP { get; set; }
        public int? PrescriptionId { get; set; }

    }
}