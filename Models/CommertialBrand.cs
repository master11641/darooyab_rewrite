using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class CommertialBrand
    {
        public int Id { get; set; }
        public int DrugId { get; set; }
        public string DrugName { get; set; }
        public string PersTranslateDrugName { get; set; }
        public long? Irc { get; set; }
        public short? FactoryId { get; set; }
        public short? ImporterCompanyId { get; set; }
        public int? TblBrandAttributeRank { get; set; }
        public string ProductInCountries { get; set; }
        public byte? ProducingCountryId { get; set; }
        public string ProductBy { get; set; }
        public short? FactoryProducterId { get; set; }
        public string ImporterBy { get; set; }
        public short? FactoryImporterId { get; set; }
    }
}
