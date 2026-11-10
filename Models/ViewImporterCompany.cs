using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class ViewImporterCompany
    {
        public int? ProductNumber { get; set; }
        public short? ImporterCompanyId { get; set; }
        public string FactoryName { get; set; }
        public string FactoryTypeName { get; set; }
    }
}
