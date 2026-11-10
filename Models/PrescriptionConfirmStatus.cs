using System.ComponentModel.DataAnnotations;

namespace darooyab.Models
{
    public class PrescriptionConfirmStatus
    {
        public PrescriptionConfirmStatus()
        {
            PrescriptionConfirms = new HashSet<PrescriptionConfirm>();
        }
        public int Id { get; set; }
        [Display(Name ="عنوان")]
        public string Title { get; set; }
         [Display(Name ="توضیحات اجباری باشد")]
        public bool DescriptionRequired { get; set; }
        public ICollection<PrescriptionConfirm> PrescriptionConfirms { get; set; }
    }
}
