using DNTPersianUtils.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using darooyab_rewrite.Utilities;

namespace darooyab.Models
{
    public enum TransactionType : Int16
    {
        Cach = 0,
        Gift = 1

    }
    public class WalletTransaction
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="تکمیل فیلد مبلغ الزامی است")]
        public int AccountId { get; set; }      
        public DateTime? RegisterDate { get; set; } = DateTime.Now;
        public DateTime? PaymentDate { get; set; }
        [NotMapped]
        public string PersianRegisterDate
        {
            get
            {
                return PersianDateFormatter.Format(this.RegisterDate);
            }
            set
            {
                this.RegisterDate =string.IsNullOrEmpty(value) ? DateTime.Now :   (DateTime)value.ToGregorianDateTime();
            }
        }
        [NotMapped]
        public string PersianPaymentDate
        {
            get
            {
                return PersianDateFormatter.Format(this.PaymentDate);
            }
            set
            {
                this.PaymentDate = string.IsNullOrEmpty(value) ? DateTime.Now : (DateTime)value.ToGregorianDateTime() ;
            }
        }

        public int? Amount { get; set; }
        public string? Authority { get; set; }
        public string? RefId { get; set; }
        public TransactionType? TransactionType { get; set; }

        public string? Description { get; set; }
        public bool? CheckedScheduler { get; set; }
        public string? ResultScheduler { get; set; }
    }

}
