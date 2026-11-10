using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class PrescriptionConfirm
    {
        public int Id { get; set; }
        public DateTime ConfirmDate { get; set; }
        public int StatusId { get; set; }
        [ForeignKey("StatusId")]
        public PrescriptionConfirmStatus PrescriptionConfirmStatus { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public DarooyabUser DarooyabUser { get; set; }
        public int PrescriptionId { get; set; }
        [ForeignKey("PrescriptionId")]
        public DarooyabRecommandPrescription DarooyabRecommandPrescription { get; set; }
        public string? Description { get; set; }

    }
}
