

namespace darooyab.Models
{
    public class DarooyabUrl
    {
        public DarooyabUrl()
        {
            DarooyabUrlRoles = new HashSet<DarooyabUrlRole>();
        }
        public int Id { get; set; }

        public string Url { get; set; }
        public string? Area { get; set; }
        public string Controller { get; set; }
        public string? Action { get; set; }
        public string? Description { get; set; }
        public string? Title { get; set; }
        public int? SortIndex { get; set; }
        public ICollection<DarooyabUrlRole> DarooyabUrlRoles { get; set; }
    }

}