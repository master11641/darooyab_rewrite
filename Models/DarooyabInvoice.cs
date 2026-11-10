using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class DarooyabInvoice
    {
        public DarooyabInvoice()
        {

        }
        public int Id { get; set; }
        public string? RefId { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public long Amount { get; set; }
        public string Authority { get; set; }
        public bool IsDeleted { get; set; }
        public string? Description { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public DarooyabUser User { get; set; }
        public int PackageId { get; set; }
        [ForeignKey("PackageId")]
        public DarooyabPackage DarooyabPackage { get; set; }
        public int? DiscountId { get; set; }
        [ForeignKey("DiscountId")]
        public DarooyabDiscount? DarooyabDiscount { get; set; }
        public bool? CheckedScheduler { get; set; }
        public string? ResultScheduler { get; set; }
    }
}