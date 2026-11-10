namespace darooyab.Models
{
    public class DarooyabSuperService
    {
        public long ServiceId { get; set; }
        public int SuperSpecialityId { get; set; }
        public DarooyabSuperSpecialty DarooyabSuperSpecialty { get; set; }
        public DarooyabService DarooyabService { get; set; }
    }
}