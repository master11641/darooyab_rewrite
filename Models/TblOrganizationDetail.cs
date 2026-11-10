using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class TblOrganizationDetail
    {
        public int TblOrganizationDetailId { get; set; }
        public int? TblOrganizationDetailFactoryIdFk { get; set; }
        public string TblOrganizationDetailLogoAddress { get; set; }
        public string TblOrganizationDetailSiteAddress { get; set; }
        public string TblOrganizationDetailEmail { get; set; }
        public string TblOrganizationDetailTel { get; set; }
        public string TblOrganizationDetailFax { get; set; }
        public string TblOrganizationDetailAddress { get; set; }
        public int? TblOrganizationDetailRankInDarooyab { get; set; }
        public string TblOrganizationDetailIntroduction { get; set; }
    }
}
