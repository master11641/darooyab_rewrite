using System.ComponentModel.DataAnnotations.Schema;
using darooyab.Models;

public class DarooyabPharmacyLocation
{
    public long Id { get; set; }
    public int CityId { get; set; }
    [ForeignKey("CityId")]
    public DarooyabCity DarooyabCity { get; set; }
    public string Title { get; set; }
    public string Phone { get; set; }
    public long ServiceId { get; set; }
    [ForeignKey("ServiceId")]
    public DarooyabService? DarooyabService { get; set; }
    public int UserId { get; set; }
    
    [ForeignKey("UserId")]
    public DarooyabUser? DarooyabUser { get; set; }
    public bool? IsDeleted { get; set; }
}