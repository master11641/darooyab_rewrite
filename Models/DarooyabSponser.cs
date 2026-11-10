using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models
{
    public enum SponserType
    {
        [Display(Name = "اسپانسر")]
        Sponser,
        [Display(Name = "درباره ما")]
        About,
        [Display(Name = "منابع و گروه علمی")]
        Resource


    }
    public class DarooyabSponser
    {
        public int Id { get; set; }
        [Display(Name = "نوع اسپانسر")]
        public SponserType SponserType { get; set; }
        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "تکمیل عنوان اسپانسر الزامی است")]
        public string Title { get; set; }
        [Display(Name = "توضیحات")]
        [UIHint("Editor")]
        public string? Description { get; set; }

    }
}