using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class NewsInCategory
    {
        public int Id { get; set; }
        public int CatId { get; set; }
        public int NewsId { get; set; }
    }
}
