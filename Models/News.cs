using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class News
    {
        public int NewsId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public string Pic { get; set; }
        public string NewsType { get; set; }
        public string Summary { get; set; }
    }
}
