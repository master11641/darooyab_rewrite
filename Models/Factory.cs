using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class Factory
    {
        public Factory()
        {
            AppUsersCompanies = new HashSet<AppUsersCompany>();
            DrugsNameFactories = new HashSet<DrugsName>();
            DrugsNameFactoryId2Navigations = new HashSet<DrugsName>();
            DrugsNameImporterCompanies = new HashSet<DrugsName>();
            WebSiteUserAccounts = new HashSet<WebSiteUserAccount>();
            DarooyabServices = new HashSet<DarooyabService>();
            DarooyabAppNewses = new HashSet<DarooyabAppNews>();
            DarooyabDrugDistributions = new HashSet<DarooyabDrugDistribution>();
            DarooyabUserFactories = new HashSet<DarooyabUserFactory>();
        }

        public short FactoryId { get; set; }
        public string? FactoryName { get; set; }
        public string? AddrssLink { get; set; }
        public string? TestArabic { get; set; }
        public byte? CountryId { get; set; }
        public byte? FactoryTypeId { get; set; }
        public int? Province { get; set; }

        public virtual Country? Country { get; set; }
        public virtual FactoryType? FactoryType { get; set; }
        public virtual ICollection<AppUsersCompany> AppUsersCompanies { get; set; }
        public virtual ICollection<DrugsName> DrugsNameFactories { get; set; }
        public virtual ICollection<DrugsName> DrugsNameFactoryId2Navigations { get; set; }
        public virtual ICollection<DrugsName> DrugsNameImporterCompanies { get; set; }
        public virtual ICollection<WebSiteUserAccount> WebSiteUserAccounts { get; set; }
        public virtual ICollection<DarooyabService> DarooyabServices { get; set; }
        public virtual ICollection<DarooyabAppNews> DarooyabAppNewses { get; set; }
        public virtual ICollection<DarooyabDrugDistribution> DarooyabDrugDistributions { get; set; }
        public ICollection<DarooyabUserFactory> DarooyabUserFactories { get; set; }
    }
}
