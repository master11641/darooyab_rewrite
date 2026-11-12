using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models;

[Table("InsuranceProviders")]
public sealed class InsuranceProvider
{
    public const byte BaseInsuranceType = 1;
    public const byte SupplementaryInsuranceType = 2;

    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public byte InsuranceType { get; set; }
    public int OrderIndex { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreateDate { get; set; }
}
