using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DNTPersianUtils.Core;

namespace darooyab.Models
{
    public enum RegisterStatus
    {
        [Description("مشاهده شده")]
        Visited,
        [Description("ثبت شده")]
        Registered,
        [Description("دریافت درخواست توسط تامین کننده")]
        ReceivedSupplier,
        [Description("ارسال نمونه")]
        SendSample,
        [Description("دریافت نمونه")]
        ReceiptSample


    }
    public class DarooyabServiceRegister
    {
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public DarooyabUser? DarooyabUser { get; set; }
        public long ServiceId { get; set; }
        [ForeignKey("ServiceId")]
        public DarooyabService? DarooyabService { get; set; }
        /// <summary>
        /// تاریخ مشاهده نمونه در نرم افزار
        /// </summary>
        public DateTime? VisitedDate { get; set; }
        /// <summary>
        /// تاریخ ثبت درخواست
        /// </summary>
        public DateTime? RegisterDate { get; set; }


        /// <summary>
        /// تاریخ دریافت درخواست توسط تامین کننده
        /// </summary>
        public DateTime? ReceivedSupplierDate { get; set; }
        /// <summary>
        /// تاریخ ارسال نمونه
        /// </summary>
        public DateTime? SendSampleDate { get; set; }
        /// <summary>
        /// تاریخ دریافت نمونه
        /// </summary>
        public DateTime? ReceiptDate { get; set; }

        [Display(Name = "وضعیت درخواست")]
        public RegisterStatus RegisterStatus { get; set; }

        public int? AddressId { get; set; }
        [ForeignKey("AddressId")]
        public DarooyabUserAddress? DarooyabUserAddress { get; set; }

    }
}