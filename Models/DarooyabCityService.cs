

namespace darooyab.Models
{
    public class DarooyabCityService
    {
        public long ServiceId { get; set; }
        public int CityId { get; set; }
        public DarooyabCity DarooyabCity { get; set; }
        public DarooyabService DarooyabService { get; set; }
    }
}