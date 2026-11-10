using System;
using System.Collections.Generic;

namespace darooyab.Models
{
    public partial class WebSiteUserAccount
    {
        public WebSiteUserAccount()
        {
            CallNumbers = new HashSet<CallNumber>();
            CommentCommentForWebSiteAccountUserFkNavigations = new HashSet<Comment>();
            CommentReferredbyWebSiteAccountUserFkNavigations = new HashSet<Comment>();
            CommentRespByDoctorIdFkNavigations = new HashSet<Comment>();
            OfficeAddresses = new HashSet<OfficeAddress>();
        }

        public int Id { get; set; }
        /// <summary>
        /// نام
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// نام خانوادگی
        /// </summary>
        public string Family { get; set; }
        /// <summary>
        /// جنسیت
        /// </summary>
        public int GenderIdFk { get; set; }
        /// <summary>
        /// نظام پزشکی
        /// </summary>
        public string NezamCode { get; set; }
        /// <summary>
        /// نام شرکت
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// کد شرکت
        /// </summary>
        public short? FactoryIdFk { get; set; }
        /// <summary>
        /// نوع فعالیت
        /// </summary>
        public int ActivityIdFk { get; set; }
        /// <summary>
        /// نوع تخصص
        /// </summary>
        public int? SpecialtyIdFk { get; set; }
        /// <summary>
        /// شرح کوتاه تخصص
        /// </summary>
        public string SpecialtyShortDescription { get; set; }
        /// <summary>
        /// شرح کامل فعالیت
        /// </summary>
        public string ActivitiyFullDescription { get; set; }
        /// <summary>
        /// کد استان
        /// </summary>
        public int? ProvinceIdFk { get; set; }
        /// <summary>
        /// کد شهر
        /// </summary>
        public int? CityIdFk { get; set; }
        /// <summary>
        /// آدرس
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// آدرس عکس صورت
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// تمایل پاسخ به سوالات بیمار را دارید
        /// </summary>
        public bool? IsansweringPatientQuestion { get; set; }
        /// <summary>
        /// شماره تلفن اختصاصی(نام کاربری) 
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// کلمه عبور
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// تایید ثبت نام توسط مدیر
        /// </summary>
        public bool? IsactiveByAdmin { get; set; }
        public string InstagramUrl { get; set; }
        public string Whatsapp { get; set; }
        public int? Rank { get; set; }

        public virtual Category ActivityIdFkNavigation { get; set; }
        public virtual Category CityIdFkNavigation { get; set; }
        public virtual Factory FactoryIdFkNavigation { get; set; }
        public virtual Category GenderIdFkNavigation { get; set; }
        public virtual Category ProvinceIdFkNavigation { get; set; }
        public virtual Category SpecialtyIdFkNavigation { get; set; }
        public virtual ICollection<CallNumber> CallNumbers { get; set; }
        public virtual ICollection<Comment> CommentCommentForWebSiteAccountUserFkNavigations { get; set; }
        public virtual ICollection<Comment> CommentReferredbyWebSiteAccountUserFkNavigations { get; set; }
        public virtual ICollection<Comment> CommentRespByDoctorIdFkNavigations { get; set; }
        public virtual ICollection<OfficeAddress> OfficeAddresses { get; set; }
    }
}
