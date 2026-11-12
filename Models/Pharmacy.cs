using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class Pharmacy
    {
        public Pharmacy()
        {
            PharmacyAccounts = new HashSet<PharmacyAccount>();
            RequestMedications = new HashSet<RequestMedication>();
            DarooyabFavoritePharmacies=new HashSet<DarooyabFavoritePharmacy>();
            PharmacySpecialtyMaps = new HashSet<PharmacySpecialtyMap>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int ProvinceIdFk { get; set; }
        public int CityIdFk { get; set; }
        public int UniversityIdFk { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public int PharmacyNatureFromCategoryIdFk { get; set; }
        public int PharmacyTypeFromCategoryIdFk { get; set; }
        public string? Phone { get; set; }
        public string? PharmacyManagerMobile { get; set; }
        public string? InventoryManagerMobile { get; set; }
        public string? PharmacyTechnicianMobile { get; set; }
        public string? Introduction { get; set; }
        public string? PharmacyEngName { get; set; }
        public bool? DarooyabAdminApproved { get; set; }
        public string? Image1Url { get; set; }
        public string? Image2Url { get; set; }
        public string? Image3Url { get; set; }
        public string? SecondaryPhone { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }     
        public int? DistrictId { get; set; }

        /// <summary>
        /// |روزانه
        /// |نیمه وقت|
        /// شبانه روزی
        /// </summary>
        public int? PharmacyActivityTypeFromCategoryIdFk { get; set; }

        public virtual Category CityIdFkNavigation { get; set; }
        public virtual Category ProvinceIdFkNavigation { get; set; }
        public virtual ICollection<PharmacyAccount> PharmacyAccounts { get; set; }
        public virtual ICollection<RequestMedication> RequestMedications { get; set; }
        public virtual ICollection<DarooyabFavoritePharmacy> DarooyabFavoritePharmacies { get; set; }
        public virtual ICollection<PharmacySpecialtyMap> PharmacySpecialtyMaps { get; set; }
    }
}
