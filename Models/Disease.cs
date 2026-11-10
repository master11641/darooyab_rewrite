using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class Disease
    {
        /// <summary>
        /// کلید اصلی
        /// </summary>
        public int DiseaseId { get; set; }
        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime InsertDate { get; set; }
        /// <summary>
        /// تاریخ بروزرسانی
        /// </summary>
        public DateTime EditDate { get; set; }
        /// <summary>
        /// نام فارسی بیماری
        /// </summary>
        public string DiseaseNameFa { get; set; }
        /// <summary>
        /// نام انگلیسی بیماری
        /// </summary>
        public string DiseaseNameEn { get; set; }
        /// <summary>
        /// عنوان صفحه (title)
        /// </summary>
        public string DiseaseTitle { get; set; }
        /// <summary>
        /// شرح کوتاه بیماری (Description)
        /// </summary>
        public string DiseaseShortDescription { get; set; }
        /// <summary>
        /// توضیحات کامل بیماری
        /// </summary>
        public string DiseaseFullDescription { get; set; }
        /// <summary>
        /// کد گروه مرتبط مارتیندل
        /// </summary>
        public byte? GroupIdFk { get; set; }

        public virtual Group1 GroupIdFkNavigation { get; set; }
    }
}
