
namespace darooyab.Models
{
    public class DarooyabGeneralService
    {
        public long ServiceId { get; set; }
        public int GeneralId { get; set; }
        public DarooyabGeneralItem DarooyabGeneralItem { get; set; }
        public DarooyabService DarooyabService { get; set; }
    }
}