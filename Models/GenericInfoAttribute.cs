using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class GenericInfoAttribute
    {
        public int Id { get; set; }
        public int? DrugId { get; set; }
        public string? Ahfscode { get; set; }
        public string? GeneralInformation { get; set; }
        public int? ToxicationIdFk { get; set; }

        public virtual TblToxication? ToxicationIdFkNavigation { get; set; }
    }
}
