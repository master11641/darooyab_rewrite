namespace darooyab.Models
{
    public class DarooyabCityDistrict
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Int16 OrderIndex { get; set; }
        public bool IsActive { get; set; }
        public int CityId { get; set; }
    }
}
