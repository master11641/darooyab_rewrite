using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class DarooyabUserRole
    {
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public DarooyabUser DarooyabUser { get; set; }
        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public DarooyabRole DarooyabRole { get; set; }
    }
}
