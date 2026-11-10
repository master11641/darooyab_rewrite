using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class TblNews
    {
        public int TblNewsId { get; set; }
        public string TblNewsTitle { get; set; }
        public string TblNewsSubTitle { get; set; }
        public string TblNewsShortDescription { get; set; }
        public string TblNewsLongDescription { get; set; }
        public string TblNewsImageDirectory { get; set; }
        public bool TblNewsIsActive { get; set; }
        /// <summary>
        /// خبر برگزیده است ؟
        /// </summary>
        public bool? TblNewsIsFavorite { get; set; }
        /// <summary>
        /// مدیر
        /// </summary>
        public bool? TblNewsDoConfirmByManager { get; set; }
        public bool TblNewsTypeIsPage { get; set; }
        public DateTime? TblNewsRegisterDate { get; set; }
        public DateTime? TblNewsExpireDate { get; set; }
        public int TblNewsGroupIdFk { get; set; }
        /// <summary>
        /// شناسه خارجی دارو
        /// </summary>
        public int? TblDrugsNameIdFk { get; set; }
        /// <summary>
        /// شناسه خارجی برند (نام تجاری)
        /// </summary>
        public int? TblDrugNameIrcidFk { get; set; }
        /// <summary>
        /// شناسه خارجی شرکت
        /// </summary>
        public int? TblFactorysIdFk { get; set; }
        /// <summary>
        /// شناسه خارجی سازمان
        /// </summary>
        public int? TblCategoryIdFk { get; set; }
        /// <summary>
        /// شناسه خارجی گروه دارویی
        /// </summary>
        public int? TblGroupsIdFk { get; set; }

        public virtual TblNewsGroup TblNewsGroupIdFkNavigation { get; set; }
    }
}
