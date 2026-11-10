using System.ComponentModel.DataAnnotations;

public class CompanyReservedGeneric
{
    public int Id { get; set; }
    [Display(Name ="نام ژنریک")]
    public int DrugID { get; set; }
       [Display(Name ="شرکت دارویی")]
    public Int16 FactoryID { get; set; }
       [Display(Name ="اپلیکیشن")]
    public bool Appilcation { get; set; }
       [Display(Name ="وب سایت")]
    public bool WebSite { get; set; }
       [Display(Name ="تاریخ شروع")]
    public DateTime ReservationStartDate { get; set; }
       [Display(Name ="تاریخ پایان")]
    public DateTime ReservationEndDate { get; set; }
       [Display(Name ="تاریخ ثبت")]
    public DateTime CreatedDate { get; set; }
       [Display(Name ="توضیح")]
    [UIHint("CKEditor")]
    public string Description { get; set; }

}