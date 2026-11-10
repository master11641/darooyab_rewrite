using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class DarooyabDiscount
    {
        public DarooyabDiscount()
        {
            DarooyabInvoices = new HashSet<DarooyabInvoice>();
        }
        public int Id { get; set; }
        public string DiscountCode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpireDate { get; set; }
        public int DiscountPercent { get; set; }

        public ICollection<DarooyabInvoice> DarooyabInvoices { get; set; }
    }
}