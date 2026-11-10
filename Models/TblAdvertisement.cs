using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class TblAdvertisement
    {
        public int AdvId { get; set; }
        public bool? AdvIsActive { get; set; }
        public bool? AdvIsPublic { get; set; }
        public DateTime? AdvStartDate { get; set; }
        public DateTime? AdvEndDate { get; set; }
        public string AdvImageUrl { get; set; }
        public string AdvImageAlt { get; set; }
        public string AdvLink { get; set; }
        public int? AdvRank { get; set; }
        public string AdvBayer { get; set; }
        public int? AdvDrugIdFk { get; set; }
        public int? AdvFactoryIdFk { get; set; }
        public int? AdvGroupIdFk { get; set; }
        public int? AdvDrugIdSearchFk { get; set; }
        public DateTime? AdvEditDate { get; set; }
        public string AdvSaveUser { get; set; }
        public string AdvEditUser { get; set; }
    }
}
