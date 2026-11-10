using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class Suggestion
    {
        public int Id { get; set; }
        public Guid? UserId { get; set; }
        public string Suggestion1 { get; set; }
        public DateTime? SentDate { get; set; }
    }
}
