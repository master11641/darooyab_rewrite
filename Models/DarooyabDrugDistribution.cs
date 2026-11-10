using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using darooyab.Models;
using System.ComponentModel.DataAnnotations.Schema;
using DNTPersianUtils.Core;
using darooyab_rewrite.Utilities;
/// <summary>
/// مدل مربوط به جدول اعلام توزیع دارو
/// </summary>
public class DarooyabDrugDistribution
{
    public int Id { get; set; }
    [Display(Name = "نام فارسی برند")]
    public int DrugsNameId { get; set; }
    [ForeignKey("DrugsNameId")]
    [Display(Name = "نام فارسی برند")]
    public virtual DrugsName? DrugsName { get; set; }
    [Display(Name = "شرکت توزیع")]
    public short FactoryId { get; set; }
    [ForeignKey("FactoryId")]
    [Display(Name = "شرکت توزیع")]
    public virtual Factory? Factory { get; set; }
    [UIHint("PersianDatePicker")]
    [Display(Name = "تاریخ اعلام")]
    public DateTime AnnouncementDate { get; set; } = DateTime.Now;
    [Display(Name = "تاریخ شروع")]
    [NotMapped]
    public string PersianAnnouncementDate
    {
        get
        {
            return PersianDateFormatter.Format(this.AnnouncementDate);
        }
        set
        {
            this.AnnouncementDate = (DateTime)value.ToGregorianDateTime();
        }
    }
    /// <summary>
    /// تاریخ درج رکورد
    /// </summary>
    /// <value></value>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime InsertDate { get; set; } = DateTime.Now;
    [Display(Name = "توضیحات")]
    [UIHint("CKEditor")]
    public string? Description { get; set; }
}
