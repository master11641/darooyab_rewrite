using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class PharmacySpecialtyMap
    {
        [Key]
        public int ID { get; set; }

        public int PharmacyId_fk { get; set; }
        public int PharmacySpecialtyId_fk { get; set; }
        [ForeignKey("PharmacyId_fk")]
        public Pharmacy Pharmacy { get; set; }
        [ForeignKey("PharmacySpecialtyId_fk")]
        public PharmacySpecialty PharmacySpecialty { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

        public DateTime? CreateDate { get; set; }
    }
}
