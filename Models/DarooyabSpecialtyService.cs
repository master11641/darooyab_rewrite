namespace darooyab.Models
{
    public class DarooyabSpecialtyService
    {
        public long ServiceId { get; set; }
        public int SpecialtyId { get; set; }
        public DarooyabSpecialty DarooyabSpecialty { get; set; }
        public DarooyabService DarooyabService { get; set; }
    }
}