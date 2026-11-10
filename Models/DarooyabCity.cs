using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class DarooyabCity
    {
        public DarooyabCity()
        {
            DarooyabUserAddresses = new HashSet<DarooyabUserAddress>();
            DarooyabCityServices = new HashSet<DarooyabCityService>();
            PharmacyLocations = new HashSet<DarooyabPharmacyLocation>();
             DarooyabDrugShortages=new HashSet<DarooyabDrugShortage>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderIndex { get; set; }
        public bool IsActive { get; set; }
        public int ProvinceId { get; set; }
        [ForeignKey("ProvinceId")]
        public DarooyabProvince DarooyabProvince { get; set; }
        public ICollection<DarooyabUserAddress> DarooyabUserAddresses { get; set; }
        public ICollection<DarooyabCityService> DarooyabCityServices { get; set; }
        public ICollection<DarooyabPharmacyLocation> PharmacyLocations { get; set; }
        public ICollection<DarooyabDrugShortage> DarooyabDrugShortages { get; set; }

    }
}