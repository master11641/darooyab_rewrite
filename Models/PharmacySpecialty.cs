using System.ComponentModel.DataAnnotations;

namespace darooyab.Models
{

    public class PharmacySpecialty
    {
        public PharmacySpecialty()
        {
            PharmacySpecialtyMaps = new HashSet<PharmacySpecialtyMap>();
        }
        [Key]
        public int ID { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public int OrderIndex { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }
        public virtual ICollection<PharmacySpecialtyMap> PharmacySpecialtyMaps { get; set; }
    }
}

