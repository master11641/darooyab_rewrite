using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class DarooyabPackage
    {
        public DarooyabPackage()
        {
            DarooyabInvoices = new HashSet<DarooyabInvoice>();
            DarooyabUserPackageFrees = new HashSet<DarooyabUserPackageFree>();

        }
        public int Id { get; set; }
        public string Title { get; set; }
        public long Amount { get; set; }
        public int ValidityDurationInDays { get; set; }
        public string ImageUrl { get; set; }
        public ICollection<DarooyabInvoice> DarooyabInvoices { get; set; }
        public ICollection<DarooyabUserPackageFree> DarooyabUserPackageFrees { get; set; }

    }
}