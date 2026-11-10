using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class PharmacyRequestLocal
    {
        public PharmacyRequestLocal()
        {
            // PharmacyExchangeExcessPhReqIdFkNavigations = new HashSet<PharmacyExchange>();
            // PharmacyExchangeShortagePhReqIdFkNavigations = new HashSet<PharmacyExchange>();
        }

        public int Id { get; set; }
        public int? PharmacyAccountIdFk { get; set; }
        public int? RequestTypeFromCategoryIdFk { get; set; }
        public DateTime? RequestExpireDate { get; set; }
        public int? DrugCount { get; set; }
        public DateTime? DrugExpireDate { get; set; }
        public double? UnitPrice { get; set; }
        public string? PatchNumber { get; set; }
        public string? PurchaseInvoiceNumber { get; set; }
        public int? CompanyDistributorIdFk { get; set; }
        public DateTime? RegistrationDateTime { get; set; }
        public string? Description { get; set; }
        public long? Irc { get; set; }
        public int? DrugShapFromDosageIdFk { get; set; }
        /// <summary>
        /// تعداد محدود - 1/نامحدود - 2
        /// </summary>
        public int? CountLimit { get; set; }
        /// <summary>
        /// ارجاع بیمار-قابل مشاهده برای
        /// </summary>
        public string? PationRefferalView { get; set; }
        /// <summary>
        /// لوکس-بدون محدودیت
        /// </summary>
        public string? ExchangeLimitNum { get; set; }
        /// <summary>
        /// فروش نقدی به همکار
        /// </summary>
        public bool? SellToColleague { get; set; }
        /// <summary>
        /// قیمت فروش به همکار
        /// </summary>
        public double? ColleaguePrice { get; set; }
        /// <summary>
        /// تبادل دارویی-قابل مشاهده برای
        /// </summary>
        public int? RequestView { get; set; }
        public string? DrugUnitNum { get; set; }
        public int? ExchangeRandomCode { get; set; }
        public string? StatusNum { get; set; }
        public int? AddedByFdoaccountIdFk { get; set; }
        public bool? IsHiddenByFdo { get; set; }

        // public virtual PharmacyAccount? AddedByFdoaccountIdFkNavigation { get; set; }
        // public virtual DosageForm? DrugShapFromDosageIdFkNavigation { get; set; }
        // public virtual PharmacyAccount? PharmacyAccountIdFkNavigation { get; set; }
        // public virtual Category? RequestTypeFromCategoryIdFkNavigation { get; set; }
        // public virtual ICollection<PharmacyExchange> PharmacyExchangeExcessPhReqIdFkNavigations { get; set; }
        // public virtual ICollection<PharmacyExchange> PharmacyExchangeShortagePhReqIdFkNavigations { get; set; }
    }
}
