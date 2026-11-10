using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class DarooyabActivityMedium
    {
        /// <summary>
        /// شناسه جدول
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// شناسه جدول فعالیت
        /// </summary>
        [Display(Name = "شناسه جدول فعالیت")]
        public int ActivityId { get; set; }
        /// <summary>
        /// نام فایل کلیپ
        /// </summary>
        [Display(Name = "فایل کلیپ")]
        [UIHint("Upload")]
        public string? ClipUrl { get; set; }
        /// <summary>
        /// نام فایل تصویر اول
        /// </summary>
        [UIHint("ImageUpload")]
        [Display(Name = "تصویر اول")]
        public string? Image1Url { get; set; }
        /// <summary>
        /// نام فایل تصویر دوم
        /// </summary>
        [UIHint("ImageUpload")]
        [Display(Name = "تصویر دوم")]
        public string? Image2Url { get; set; }
        [Display(Name = "لینک کلیپ")]
        public string? ClipLink { get; set; }
        [Display(Name = "لینک تصویر اول")]
        public string? Image1Link { get; set; }
        [Display(Name = "لینک تصویر دوم")]
        public string? Image2Link { get; set; }
        [ForeignKey("ActivityId")]
        public DarooyabActivity? DarooyabActivity { get; set; }

    }
}
