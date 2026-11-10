using System.ComponentModel.DataAnnotations;

namespace darooyab.Models
{
    public class PrescriptionType
    {
        public PrescriptionType()
        {
            DarooyabRecommandPrescriptions = new List<DarooyabRecommandPrescription>();
        }
        public int Id { get; set; }
        [Display(Name ="عنوان")]
        [Required(ErrorMessage ="تکمیل عنوان الزامی است")]
        public string Title { get; set; }
        public ICollection<DarooyabRecommandPrescription> DarooyabRecommandPrescriptions { get; set; }
    }
}
