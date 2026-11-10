using System.ComponentModel.DataAnnotations.Schema;
using darooyab.Models;

public class DarooyabFavoritePharmacy
{
    
public int Id { get; set; }
public int PharmacyId { get; set; }
public int UserId { get; set; }
public DateTime CreateDate { get; set; }
public string? Description { get; set; }
[ForeignKey("PharmacyId")]
public Pharmacy pharmacy { get; set; }
[ForeignKey("UserId")]
public DarooyabUser DarooyabUser { get; set; }
    
}