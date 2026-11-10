using System.ComponentModel.DataAnnotations;

namespace darooyab.Models
{
    public class DarooyabDiseaseGroup
    {
        public DarooyabDiseaseGroup()
        {
            DarooyabDiseases = new HashSet<DarooyabDisease>();
        }
        public int Id { get; set; }
        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "تکمیل عنوان الزامی است")]
        public string Title { get; set; }
        [Display(Name = "توضیحات")]
        [UIHint("Multiline")]
        public string? Description { get; set; }
        public virtual ICollection<DarooyabDisease> DarooyabDiseases { get; set; }
    }
}
