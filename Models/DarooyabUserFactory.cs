using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class DarooyabUserFactory
    {
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public DarooyabUser DarooyabUser { get; set; }
        public short FactoryId { get; set; }
        [ForeignKey("FactoryId ")]
        public Factory Factory { get; set; }
    }

}