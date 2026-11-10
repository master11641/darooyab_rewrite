using System;

namespace darooyab.Models.DTOs
{
    public class PatientReferralGenericResult
    {
        public long ID { get; set; }
        public int PharmacyId { get; set; }
        public string? PharmacyName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime RequestExpireDate { get; set; }
        public long IRC { get; set; }
        public int TownShipId { get; set; }
        public string? TownShipName { get; set; }
        public string? ProvinceName { get; set; }
        public string? PersBrandName { get; set; }
        public string? EngBrandName { get; set; }
        public int? RankNumber { get; set; }
        public string? RankImage { get; set; }
        public string? CountLimit { get; set; }
        public int? PharmacyActivityType_FromCategoryId_fk { get; set; }
        public int TotalCount { get; set; } 

    }

    // مدل فیلترهای ورودی جهت تمیزی متدها
    public class PatientReferralFilterDto
    {
        public int GenericDrugId { get; set; }
        public int ProvId { get; set; } = -1;
        public int CityId { get; set; } = -1;
        public int DistrictId { get; set; } = -1;
        public int PharmacySpecialtyId { get; set; } = -1;
        public bool UserIsFoodAndDrug { get; set; } = false;
        public int? PharmacyAccountId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

// brand
/// <summary>
    /// مدل فیلترهای ورودی استورد پروسیجر
    /// </summary>
public class SimilarBrandFilterDto
    {
        public long BrandIrc { get; set; }
        public int ProvId { get; set; } = 0;
        public int CityId { get; set; } = 0;
        public int DistrictId { get; set; } = 0;
        public int PharmacySpecialtyId { get; set; } = 0;
        public bool UserIsFoodAndDrug { get; set; } = false;
        public int? PharmacyAccountId { get; set; } = null;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    /// <summary>
    /// مدل خروجی دقیق استورد پروسیجر sp_GetSimilarBrandPharmacyRequests_District_Speciality
    /// </summary>
    public class SimilarBrandPharmacyRequestResult
    {
         public int ID { get; set; }
        public int PharmacyId { get; set; }
        public string PharmacyName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime? RequestExpireDate { get; set; }
        public long? IRC { get; set; }
        public int? TownShipId { get; set; }
        public string? TownShipName { get; set; }
        public string? ProvinceName { get; set; }
        public Int16? RankNumber { get; set; }
        public string? RankImage { get; set; }
        public string? CountLimit { get; set; }
        public string? PersBrandName { get; set; }
        public string? EnglishBrandName { get; set; }

        // ستون‌های صفحه‌بندی برگشتی از SP
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

    public class PatientReferralSimilarBrandPagedResult
    {
        public int ID { get; set; }
        public int PharmacyId { get; set; }
        public string PharmacyName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime? RequestExpireDate { get; set; }
        public long IRC { get; set; }
        public int? TownShipId { get; set; }
        public string? TownShipName { get; set; }
        public string? ProvinceName { get; set; }
        public short? RankNumber { get; set; }
        public string? RankImage { get; set; }
        public string? CountLimit { get; set; }
        public string? PersBrandName { get; set; }
        public string? EnglishBrandName { get; set; }
        public int TotalRows { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }



}
