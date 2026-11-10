using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public class DarooyabUserPackageFree{
        // public int userId { get; set; }
        // public DarooyabUser DarooyabUser { get; set; }
        public int Id { get; set; }
        [Display(Name ="انتخاب پکیج")]
        public int PackageId { get; set; }
        [ForeignKey("PackageId")]
        public DarooyabPackage? DarooyabPackage { get; set; }
         [Display(Name ="توضیحات")]
        public string? Description { get; set; }
         [Display(Name ="شماره همراه کاربر")]
         [RegularExpression(pattern:@"^[0-9]{12}$",ErrorMessage ="شماره همراه با فرمت درست وارد کنید")]
        public string Phone { get; set; }

    }
}