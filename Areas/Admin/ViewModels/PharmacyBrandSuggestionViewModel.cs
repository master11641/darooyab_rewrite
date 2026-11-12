using System.ComponentModel.DataAnnotations;

namespace darooyab_rewrite.Areas.Admin.ViewModels;

public sealed class PharmacyBrandSuggestionViewModel
{
    [Required(ErrorMessage = "نام فارسی برند را وارد کنید.")]
    [Display(Name = "نام فارسی برند")]
    public string? PersianBrandName { get; set; }
}
