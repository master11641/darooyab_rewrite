using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class DarooyabUrlRole{
        public int UrlId { get; set; }
        [ForeignKey("UrlId")]
        public DarooyabUrl DarooyabUrl { get; set; }
        public int RoleId { get; set; }
         [ForeignKey("RoleId")]
         public DarooyabRole DarooyabRole { get; set; }
    }
    
}