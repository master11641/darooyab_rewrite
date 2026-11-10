
namespace darooyab.Models
{
    public class DarooyabProvinceService
    {
        public long ServiceId { get; set; }
        public int ProvinceId { get; set; }
        public DarooyabProvince DarooyabProvince { get; set; }
        public DarooyabService DarooyabService { get; set; }
    }
}