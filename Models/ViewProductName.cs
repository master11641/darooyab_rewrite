using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class ViewProductName
    {
        public string DrugNameEn { get; set; }
        public string DrugNameFa { get; set; }
        public short? FactoryId { get; set; }
        public int? DrugId { get; set; }
        public int? Irc { get; set; }
        public string MainGeneric { get; set; }
        public string LinkId { get; set; }
        public int? Id { get; set; }
        public byte? CountryId { get; set; }
        public short? FactoryId2 { get; set; }
        public byte? CountryId2 { get; set; }
        public string ImporterCompanyName { get; set; }
    }
}
