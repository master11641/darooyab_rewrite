using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class OfficeAddress
    {
        public int Id { get; set; }
        /// <summary>
        /// نوع: مطب/ داروخانه
        /// </summary>
        public int OfficeType { get; set; }
        /// <summary>
        /// کلید خارجی کاربر
        /// </summary>
        public int? WebSiteUserAccountIdFk { get; set; }
        public int? ProvinceIdFk { get; set; }
        public int? CityIdFk { get; set; }
        /// <summary>
        /// آدرس مطب
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// ساعت  و روز کاری مطب
        /// </summary>
        public string WorkingHoursDay { get; set; }
        /// <summary>
        /// امکانات مطب
        /// </summary>
        public string OfficeFacilities { get; set; }
        public string TelNumber1 { get; set; }
        public string TelNumber2 { get; set; }
        public string TelNumber3 { get; set; }

        public virtual Category CityIdFkNavigation { get; set; }
        public virtual Category OfficeTypeNavigation { get; set; }
        public virtual Category ProvinceIdFkNavigation { get; set; }
        public virtual WebSiteUserAccount WebSiteUserAccountIdFkNavigation { get; set; }
    }
}
