using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class TblContactu
    {
        public int TblContactusId { get; set; }
        public string TblContactusNameAndFamily { get; set; }
        public int TblContactusCategoryIdFk { get; set; }
        public string TblContactusOrganizationOrCompanyName { get; set; }
        public string TblContactusRole { get; set; }
        public string TblContactusTel { get; set; }
        public string TblContactusEmail { get; set; }
        public string TblContactusTextMessage { get; set; }
        public short? TblContactusViewByManagerStatusNum { get; set; }
        public DateTime? TblContactusViewByManagerDate { get; set; }
        public string TblContactusIpsystem { get; set; }
        public DateTime? TblContactusRegisterDate { get; set; }

        public virtual Category TblContactusCategoryIdFkNavigation { get; set; }
    }
}
