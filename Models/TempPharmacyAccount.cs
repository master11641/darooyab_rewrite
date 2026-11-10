using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class TempPharmacyAccount
    {
        public string HixCode { get; set; }
        public int ProvinceIdFk { get; set; }
        public int UniversityIdFk { get; set; }
        public string Name { get; set; }
        public int PharmacyTypeFromCategoryIdFk { get; set; }
        public int PharmacyNatureFromCategoryIdFk { get; set; }
        public int? PharmacyActivityTypeFromCategoryIdFk { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public int CityIdFk { get; set; }
        public string Phone { get; set; }
        public string FounderMobileNum { get; set; }
        public string Address { get; set; }
        public string Introduction { get; set; }
        public string PharmacyEngName { get; set; }
        public bool? PharmacyDarooyabAdminApproved { get; set; }
        public string FounderName { get; set; }
        public string FounderFamily { get; set; }
        public string PharmacyManagerName { get; set; }
        public DateTime? AccountExpireDate { get; set; }
        public bool? DarooyabAdminApproved { get; set; }
        public string FounderEmail { get; set; }
        public string AccountStatus { get; set; }
        public string NationalCode { get; set; }
        public string AdminComment { get; set; }
        public int Id { get; set; }
    }
}
