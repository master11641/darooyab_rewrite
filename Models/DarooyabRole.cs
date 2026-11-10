namespace darooyab.Models
{
    public class DarooyabRole
    {
        public DarooyabRole()
        {
             DarooyabUserRoles = new HashSet<DarooyabUserRole>();
              DarooyabUrlRoles = new HashSet<DarooyabUrlRole>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public ICollection<DarooyabUserRole> DarooyabUserRoles { get; set; }
        
        public ICollection<DarooyabUrlRole> DarooyabUrlRoles { get; set; }
    }
}
