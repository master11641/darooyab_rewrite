using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public partial class KeywordLink
    {

        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Keyword { get; set; }
        [NotMapped]
        public int DrugId { get; set; }

        [Required]
        [MaxLength(2100)]
        public string Url { get; set; }

        public bool IsFollow { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

    }
}
