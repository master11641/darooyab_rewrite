using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class Country
    {
        public Country()
        {
            DrugsNameProducingCountries = new HashSet<DrugsName>();
            DrugsNameProducingCountryId2Navigations = new HashSet<DrugsName>();
            Factories = new HashSet<Factory>();
        }

        public byte CountryId { get; set; }
        public string CountryName { get; set; }
        public string CountryNameFa { get; set; }

        public virtual ICollection<DrugsName> DrugsNameProducingCountries { get; set; }
        public virtual ICollection<DrugsName> DrugsNameProducingCountryId2Navigations { get; set; }
        public virtual ICollection<Factory> Factories { get; set; }
    }
}
