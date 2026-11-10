using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class DarooyabDisease
    {
        public DarooyabDisease()
        {
            DarooyabRecommandPrescriptions = new HashSet<DarooyabRecommandPrescription>();
        }
        public int Id { get; set; }
        [Display(Name = "نام بیماری(فارسی)")]
        public string? Title { get; set; }
        [Display(Name = "توضیحات")]
        [UIHint("CKEditor")]
        public string? Description { get; set; }
        [Display(Name = "کلمات کلیدی")]
        public string? KeyWords { get; set; }
        [Display(Name = "گروه بیماری")]

        public int? GroupId { get; set; }
        [Display(Name = "نام بیماری(انگلیسی)")]
        // [Required(ErrorMessage = "نام بیماری(انگلیسی) الزامی است")]
        public string? TitleEnglish { get; set; }
        [Display(Name = "Title Tag")]
        public string? TitleTag { get; set; }
        [Display(Name = "Description Tag")]
        [UIHint("Multiline")]
        public string? DescriptionTag { get; set; }
        [Display(Name = "موضوع")]
        public string? Subject { get; set; }

        [ForeignKey("GroupId")]
        public DarooyabDiseaseGroup? DarooyabDiseaseGroup { get; set; }
        public ICollection<DarooyabRecommandPrescription> DarooyabRecommandPrescriptions { get; set; }
    }
}
