using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace darooyab.Models;

[Table("PharmacyInsuranceContracts")]
public sealed class PharmacyInsuranceContract
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("PharmacyId_fk")]
    public int PharmacyIdFk { get; set; }

    [Column("InsuranceProviderId_fk")]
    public int InsuranceProviderIdFk { get; set; }

    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }
    public DateTime? CreateDate { get; set; }
}
