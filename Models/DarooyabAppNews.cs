using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DNTPersianUtils.Core;
using darooyab_rewrite.Utilities;

namespace darooyab.Models
{
    public partial class DarooyabAppNews
    {
        public DarooyabAppNews()
        {
            AppNewsViewLogs = new HashSet<AppNewsViewLog>();
        }

        /// <summary>
        /// کد خبر
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// کد نوع خبر
        /// </summary>
        [Display(Name = "نوع خبر")]

        public byte? NewsTypeId { get; set; }
        [Display(Name = "وضعیت فعال بودن")]
        [UIHint("IsActive")]
        public bool IsActive { get; set; }
        /// <summary>
        /// تاریخ ثبت خبر
        /// </summary>
        public DateTime? InsertDate { get; set; }
        public DateTime? ChangeDate { get; set; }
        /// <summary>
        /// تاریخ شروع نمایش بر روی اپلیکشن
        /// </summary>
        [Display(Name = "تاریخ شروع نمایش بر روی اپلیکشن")]
        public DateTime? ShowDate { get; set; }
        [NotMapped]
        public string? PersianShowDate
        {
            get
            {
                if (this.ShowDate != null)
                {
                    return PersianDateFormatter.Format(this.ShowDate);
                }
                return PersianDateFormatter.Format(DateTime.Now);
            }
            set
            {
                this.ShowDate = (DateTime)value.ToGregorianDateTime();
            }
        }
        /// <summary>
        /// عنوان خبر
        /// </summary>
        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "تکمیل عنوان الزامی است")]
        public string Title { get; set; }
        /// <summary>
        /// زیر عنوان خبر
        /// </summary>
        [Display(Name = "زیرعنوان خبر")]
        [Required(ErrorMessage = "تکمیل زیر عنوان الزامی است")]
        public string SubTitle { get; set; }
        /// <summary>
        /// متن HTML خبر
        /// </summary>
        [Display(Name = "متن خبر")]
        [UIHint("CKEditor")]
        public string? Description { get; set; }
        [Display(Name = "آدرس کلیپ")]
        [UIHint("Upload")]
        public string? ClipUrl { get; set; }
        [Display(Name = "آدرس تصویر")]
        [UIHint("ImageUpload")]
        public string? ImageUrl { get; set; }
        [Display(Name = "آدرس لوگو")]
        [UIHint("ImageUpload")]
        public string? LogoUrl { get; set; }
        [Display(Name = "IRC")]
        public long? Irc { get; set; }
        /// <summary>
        /// کد فعالیت های دریافت کننده خبر
        /// </summary>
        public string? RecipientActivities { get; set; }
        /// <summary>
        /// کد تخصص های دریافت کننده خبر
        /// </summary>
        public string? RecipientSpecialties { get; set; }
        /// <summary>
        /// کد استانهای دریافت کننده خبر
        /// </summary>
        public string? RecipientProvinces { get; set; }
        [Display(Name = "آدرس خبر")]
        [Required(ErrorMessage = "تکمیل آدرس خبر الزامی است")]
        public string? LinkUrl { get; set; }
        [Display(Name = "نوع محتوای خبر")]
        public byte? ContentTypeId { get; set; }
        [Display(Name = "شماره ترتیب")]
        public int? OrderIndex { get; set; }
        [Display(Name = "شرکت مرتبط")]
        public short? FactoryId { get; set; }
        //public DateTime? CreateDate { get; set; }
        //public DateTime? ModifyDate { get; set; }


        [Display(Name = "شرکت مرتبط")]
        [ForeignKey("FactoryId")]
        public virtual Factory? Factory { get; set; }

        public virtual ICollection<AppNewsViewLog> AppNewsViewLogs { get; set; }
    }
}
