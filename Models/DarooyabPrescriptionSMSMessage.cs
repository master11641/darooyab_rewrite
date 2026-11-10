using DNTPersianUtils.Core;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    
        public class DarooyabPrescriptionSMSMessage
    {
        public int Id { get; set; }
        public DateTime SendDate { get; set; }
        public bool? IsSuccess { get; set; }
        public int PharmacyId { get; set; }
        public int PharmacyAccountId { get; set; }
        public string Message { get; set; }
        public string RecieverPhone { get; set; }
        public int PrescriptionId { get; set; }
        [ForeignKey("PrescriptionId")]
        public DarooyabPrescription DarooyabPrescription { get; set; }
        public string StatusCode { get; set; }
        public string StatusMessage { get; set; }

    }
}