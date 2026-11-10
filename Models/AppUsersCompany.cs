using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class AppUsersCompany
    {
        public int Id { get; set; }
        /// <summary>
        /// تاریخ ثبت نام
        /// </summary>
        public DateTime RegistrationDate { get; set; }
        /// <summary>
        /// اندروید ای دی
        /// </summary>
        public string AndroidId { get; set; }
        /// <summary>
        /// نام و نام خانوادگی کاربر
        /// </summary>
        public string UserNameFamily { get; set; }
        /// <summary>
        /// کد شرکت
        /// </summary>
        public short FactoryId { get; set; }
        /// <summary>
        /// کد استان/شعبه
        /// </summary>
        public int Province { get; set; }
        /// <summary>
        /// شماره موبایل
        /// </summary>
        public string MobileNum { get; set; }
        /// <summary>
        /// کد ملی
        /// </summary>
        public string NactionalCode { get; set; }

        public virtual Factory Factory { get; set; }
        public virtual Category ProvinceNavigation { get; set; }
    }
}
