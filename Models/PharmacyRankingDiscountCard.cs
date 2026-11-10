using System.ComponentModel.DataAnnotations;

namespace darooyab.Models
{
    public enum PharmacyRankingDiscountCardStatus : byte
    {
        [Display(Name = "فعال")]
        Active = 1,
        [Display(Name = "غیرفعال")]
        Inactive = 2,
        [Display(Name = "استفاده‌شده")]
        Used = 3
    }

    public class PharmacyRankingDiscountCard
    {
        public int Id { get; set; }
        public string CardCode { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string DiscountCode { get; set; } = null!;
        public int GiftChargeAmount { get; set; }
        public short FactoryId { get; set; }
        public PharmacyRankingDiscountCardStatus Status { get; set; } = PharmacyRankingDiscountCardStatus.Active;
        public DateTime StatusDate { get; set; } = DateTime.Now;
        public int? PharmacyId { get; set; }

        public virtual Factory Factory { get; set; } = null!;
        public virtual Pharmacy? Pharmacy { get; set; }
    }
}
