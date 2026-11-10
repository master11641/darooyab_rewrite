using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class ViewCompaniesAndCountry
    {
        public string DrugName { get; set; }
        public short? FactoryId { get; set; }
        public string FactoryName { get; set; }
        public string CountryName { get; set; }
        public byte? CountryId { get; set; }
        public short? ImporterCompanyId { get; set; }
        public string ImporterCompanyName { get; set; }
    }
}
