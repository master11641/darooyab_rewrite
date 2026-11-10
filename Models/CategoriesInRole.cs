using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class CategoriesInRole
    {
        public int Id { get; set; }
        public int CatId { get; set; }
        public Guid RoleId { get; set; }
    }
}
