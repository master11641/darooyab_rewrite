using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class Category
    {
        public Category()
        {
            AppNewsViewLogs = new HashSet<AppNewsViewLog>();
            AppUsersCompanies = new HashSet<AppUsersCompany>();
            CallNumbers = new HashSet<CallNumber>();
            OfficeAddressCityIdFkNavigations = new HashSet<OfficeAddress>();
            OfficeAddressOfficeTypeNavigations = new HashSet<OfficeAddress>();
            OfficeAddressProvinceIdFkNavigations = new HashSet<OfficeAddress>();
            PharmacyCityIdFkNavigations = new HashSet<Pharmacy>();
            PharmacyProvinceIdFkNavigations = new HashSet<Pharmacy>();
            PharmacyRequests = new HashSet<PharmacyRequest>();
            RequestMedicationCityIdFkNavigations = new HashSet<RequestMedication>();
            RequestMedicationProvinceIdFkNavigations = new HashSet<RequestMedication>();
            TblBrandAttributes = new HashSet<TblBrandAttribute>();
            TblContactus = new HashSet<TblContactu>();
            WebSiteUserAccountActivityIdFkNavigations = new HashSet<WebSiteUserAccount>();
            WebSiteUserAccountCityIdFkNavigations = new HashSet<WebSiteUserAccount>();
            WebSiteUserAccountGenderIdFkNavigations = new HashSet<WebSiteUserAccount>();
            WebSiteUserAccountProvinceIdFkNavigations = new HashSet<WebSiteUserAccount>();
            WebSiteUserAccountSpecialtyIdFkNavigations = new HashSet<WebSiteUserAccount>();
        }

        public int CatId { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public int? ParentId { get; set; }
        public int? ReferenceId { get; set; }
        public string? LogoUrl { get; set; }

        public virtual ICollection<AppNewsViewLog> AppNewsViewLogs { get; set; }
        public virtual ICollection<AppUsersCompany> AppUsersCompanies { get; set; }
        public virtual ICollection<CallNumber> CallNumbers { get; set; }
        public virtual ICollection<OfficeAddress> OfficeAddressCityIdFkNavigations { get; set; }
        public virtual ICollection<OfficeAddress> OfficeAddressOfficeTypeNavigations { get; set; }
        public virtual ICollection<OfficeAddress> OfficeAddressProvinceIdFkNavigations { get; set; }
        public virtual ICollection<Pharmacy> PharmacyCityIdFkNavigations { get; set; }
        public virtual ICollection<Pharmacy> PharmacyProvinceIdFkNavigations { get; set; }
        public virtual ICollection<PharmacyRequest> PharmacyRequests { get; set; }
        public virtual ICollection<RequestMedication> RequestMedicationCityIdFkNavigations { get; set; }
        public virtual ICollection<RequestMedication> RequestMedicationProvinceIdFkNavigations { get; set; }
        public virtual ICollection<TblBrandAttribute> TblBrandAttributes { get; set; }
        public virtual ICollection<TblContactu> TblContactus { get; set; }
        public virtual ICollection<WebSiteUserAccount> WebSiteUserAccountActivityIdFkNavigations { get; set; }
        public virtual ICollection<WebSiteUserAccount> WebSiteUserAccountCityIdFkNavigations { get; set; }
        public virtual ICollection<WebSiteUserAccount> WebSiteUserAccountGenderIdFkNavigations { get; set; }
        public virtual ICollection<WebSiteUserAccount> WebSiteUserAccountProvinceIdFkNavigations { get; set; }
        public virtual ICollection<WebSiteUserAccount> WebSiteUserAccountSpecialtyIdFkNavigations { get; set; }
    }
}
