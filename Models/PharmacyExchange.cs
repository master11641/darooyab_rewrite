using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    /// <summary>
    /// جدول ثبت تبادلات داروخانه ها
    /// </summary>
    public partial class PharmacyExchange
    {
        public int Id { get; set; }
        public DateTime RegisterDatetime { get; set; }
        public int ExcessPhReqIdFk { get; set; }
        public int ShortagePhReqIdFk { get; set; }
        public int ExchangeRandomCode { get; set; }
        /// <summary>
        /// شماره فاکتور
        /// </summary>
        public string InvoiceNumber { get; set; }
        public string PatchNumber { get; set; }
        public int? CompanyDistributorIdFk { get; set; }
        public int RequireCount { get; set; }
        public string DrugUnitNum { get; set; }
        public double UnitPrice { get; set; }

        public virtual PharmacyRequest ExcessPhReqIdFkNavigation { get; set; }
        public virtual PharmacyRequest ShortagePhReqIdFkNavigation { get; set; }
    }
}
