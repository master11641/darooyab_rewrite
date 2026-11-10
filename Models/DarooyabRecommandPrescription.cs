using DNTPersianUtils.Core;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    //[ModelBinder(typeof(PersianDateModelBinder))]
    public class DarooyabRecommandPrescription
    {
        public DarooyabRecommandPrescription(){
            DarooyabMessages = new HashSet<DarooyabMessage>();
            PrescriptionConfirms = new HashSet<PrescriptionConfirm>();
        }
        public int Id { get; set; }
        [Display(Name = "عنوان")]
        public string? Title { get; set; }
        [Display(Name = "نسخه")]
        [UIHint("Editor")]
        public string? Prescription { get; set; }
        [Display(Name = "توضیحات")]
        [UIHint("Editor")]      
        public string? Description { get; set; }
        [Display(Name = "تاریخ ثبت")]
        [UIHint("PersianDatePicker")]
        public DateTime? CreateDate { get; set; }
        [NotMapped]       
        [Display(Name = "تاریخ ثبت")]
        [Required(ErrorMessage = "تکمیل تاریخ ثبت الزامی است")]
        public string? PersianCreateDate { get; set; }
        [Display(Name = "تاریخ ویرایش")]       
        public DateTime? EditDate { get; set; }
        [NotMapped]       
        [Display(Name = "تاریخ ویرایش")]
        [Required(ErrorMessage ="تکمیل تاریخ ویرایش الزامی است")]
        public string? PersianEditDate { get; set; }
        [Display(Name = "شناسه پزشک")]
        public int? UserId { get; set; }
        [Display(Name = "عنوان پزشک")]
        public string? UserTitle { get; set; }
        [Display(Name = "بیماری")]
        public int DiseaseId { get; set; }
        [ForeignKey("UserId")]
        public DarooyabUser? DarooyabUser { get; set; }
        [ForeignKey("DiseaseId")]
        public DarooyabDisease? DarooyabDisease { get; set; }
        [Display(Name = "نوع نسخه")]
        public int PrescriptionTypeId { get; set; }
        [ForeignKey("PrescriptionTypeId")]
        [Display(Name = "نوع نسخه")]
        public PrescriptionType? PrescriptionType { get; set; }
        public ICollection<DarooyabMessage> DarooyabMessages { get; set; }
        public ICollection<PrescriptionConfirm> PrescriptionConfirms { get; set; }
    }
}
