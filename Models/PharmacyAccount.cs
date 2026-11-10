using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class PharmacyAccount
    {
        public PharmacyAccount()
        {
            PharmacyRequestAddedByFdoaccountIdFkNavigations = new HashSet<PharmacyRequest>();
            PharmacyRequestPharmacyAccountIdFkNavigations = new HashSet<PharmacyRequest>();
        }

        public int Id { get; set; }
        public int PharmacyIdFk { get; set; }
        public string FounderName { get; set; }
        public string? FounderFamily { get; set; }
        public string? FounderMobileNum { get; set; }
        public string? PharmacyManagerName { get; set; }
        public string? PharmacyManagerMobile { get; set; }
        public DateTime? AccountExpireDate { get; set; }
        public bool? DarooyabAdminApproved { get; set; }
        public string? PasswordHash { get; set; }
        public string? FounderEmail { get; set; }
        public string AccountStatus { get; set; }
        public string? NationalCode { get; set; }
        public string? HixCode { get; set; }
        public string? AdminComment { get; set; }
        public bool? IsAbilityToEdit { get; set; }

        public virtual Pharmacy PharmacyIdFkNavigation { get; set; }
        public virtual ICollection<PharmacyRequest> PharmacyRequestAddedByFdoaccountIdFkNavigations { get; set; }
        public virtual ICollection<PharmacyRequest> PharmacyRequestPharmacyAccountIdFkNavigations { get; set; }
    }
}
