using System.Data;
using darooyab.Models.DTOs;
using darooyab.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace darooyab.Models
{
    public static class DarooConContextExtensions
    {
        public static async Task<List<UserDarooyabHistoryResult>> GetUserDarooyabHistoriesAsync(
            this darooConContext context,
            int userId,
            int offset = 0,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var userIdParam = new SqlParameter("@UserId", userId);
            var offsetParam = new SqlParameter("@Offset", offset);
            var pageSizeParam = new SqlParameter("@PageSize", pageSize);

            return await context
                .Set<UserDarooyabHistoryResult>()
                .FromSqlRaw(
                    "EXEC [dbo].[GetUserDarooyabHistories] @UserId, @Offset, @PageSize",
                    userIdParam,
                    offsetParam,
                    pageSizeParam)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        public static async Task<List<UserDarooyabHistoryResult>> GetUserDarooyabHistoriesAsync(
    this darooConContext context,
    int userId,
    int offset = 0,
    int pageSize = 10)
        {
            return await context
                .Set<UserDarooyabHistoryResult>()
                .FromSqlRaw(
                    "EXEC [dbo].[GetUserDarooyabHistories] @UserId, @Offset, @PageSize",
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@Offset", offset),
                    new SqlParameter("@PageSize", pageSize))
                .AsNoTracking()
                .ToListAsync();
        }


            /// <summary>
        /// اجرای پروسیجر GetPatientReferralByGeneric_District_Speciality با پشتیبانی از صفحه‌بندی
        /// </summary>
        public static async Task<List<PatientReferralGenericResult>> GetPatientReferralsAsync(
            this darooConContext context,
            int genericDrugId,
            int provId = -1,
            int cityId = -1,
            int districtId = -1,
            int pharmacySpecialtyId = -1,
            bool userIsFoodAndDrug = false,
            int? pharmacyAccountId = null,
            int pageNumber = 1,
            int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var pGenericDrugId = new SqlParameter("@genericDrugId", SqlDbType.Int) { Value = genericDrugId };
            var pProvId = new SqlParameter("@provId", SqlDbType.Int) { Value = provId };
            var pCityId = new SqlParameter("@cityId", SqlDbType.Int) { Value = cityId };
            var pDistrictId = new SqlParameter("@districtId", SqlDbType.Int) { Value = districtId };
            var pPharmacySpecialtyId = new SqlParameter("@pharmacySpecialtyId", SqlDbType.Int) { Value = pharmacySpecialtyId };
            var pUserIsFoodAndDrug = new SqlParameter("@userIsFoodAndDrug", SqlDbType.Bit) { Value = userIsFoodAndDrug };
            var pPharmacyAccountID = new SqlParameter("@pharmacyAccountID", SqlDbType.Int) 
            { 
                Value = (object?)pharmacyAccountId ?? DBNull.Value 
            };
            var pPageNumber = new SqlParameter("@PageNumber", SqlDbType.Int) { Value = pageNumber };
            var pPageSize = new SqlParameter("@PageSize", SqlDbType.Int) { Value = pageSize };

            var sql = @"EXEC [dbo].[GetPatientReferralByGeneric_District_Speciality_Pagination] 
                        @genericDrugId, 
                        @provId, 
                        @cityId, 
                        @districtId, 
                        @pharmacySpecialtyId, 
                        @userIsFoodAndDrug, 
                        @pharmacyAccountID, 
                        @PageNumber, 
                        @PageSize";

            return await context.PatientReferralGenericResults
                .FromSqlRaw(sql, 
                    pGenericDrugId, 
                    pProvId, 
                    pCityId, 
                    pDistrictId, 
                    pPharmacySpecialtyId, 
                    pUserIsFoodAndDrug, 
                    pPharmacyAccountID, 
                    pPageNumber, 
                    pPageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
         /// <summary>
        /// اورلود با دریافت شیء فیلتر (DTO) جهت سهولت در استفاده
        /// </summary>
        public static Task<List<PatientReferralGenericResult>> GetPatientReferralsAsync(
            this darooConContext context,
            PatientReferralFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            return context.GetPatientReferralsAsync(
                filter.GenericDrugId,
                filter.ProvId,
                filter.CityId,
                filter.DistrictId,
                filter.PharmacySpecialtyId,
                filter.UserIsFoodAndDrug,
                filter.PharmacyAccountId,
                filter.PageNumber,
                filter.PageSize,
                cancellationToken
            );
        }


        /// <summary>
        /// دریافت مستقیم لیست رکوردهای برند مشابه
        /// </summary>
        public static async Task<List<SimilarBrandPharmacyRequestResult>> GetSimilarBrandPharmacyRequestsAsync(
            this darooConContext context,
            SimilarBrandFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            var parameters = new[]
            {
                new SqlParameter("@brandIrc", SqlDbType.BigInt) { Value = filter.BrandIrc },
                new SqlParameter("@provId", SqlDbType.Int) { Value = filter.ProvId },
                new SqlParameter("@cityId", SqlDbType.Int) { Value = filter.CityId },
                new SqlParameter("@districtId", SqlDbType.Int) { Value = filter.DistrictId },
                new SqlParameter("@pharmacySpecialtyId", SqlDbType.Int) { Value = filter.PharmacySpecialtyId },
                new SqlParameter("@userIsFoodAndDrug", SqlDbType.Bit) { Value = filter.UserIsFoodAndDrug },
                new SqlParameter("@pharmacyAccountID", SqlDbType.Int)
                {
                    Value = (object?)filter.PharmacyAccountId ?? DBNull.Value
                },
                new SqlParameter("@PageNumber", SqlDbType.Int) { Value = filter.PageNumber },
                new SqlParameter("@PageSize", SqlDbType.Int) { Value = filter.PageSize }
            };

            const string sql = @"
                EXEC [dbo].[GetDarooyabPatientReferralByBrand_District_Speciality_Pagination]
                    @brandIrc,
                    @provId,
                    @cityId,
                    @districtId,
                    @pharmacySpecialtyId,
                    @userIsFoodAndDrug,
                    @pharmacyAccountID,
                    @PageNumber,
                    @PageSize";

            return await context.Set<SimilarBrandPharmacyRequestResult>()
                .FromSqlRaw(sql, parameters)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public static async Task<List<PatientReferralSimilarBrandPagedResult>> GetSimilarBrandPatientReferralsAsync(
            this darooConContext context,
            SimilarBrandFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(filter);

            var parameters = new[]
            {
                new SqlParameter("@brandIrc", SqlDbType.BigInt) { Value = filter.BrandIrc },
                new SqlParameter("@provId", SqlDbType.Int) { Value = filter.ProvId },
                new SqlParameter("@cityId", SqlDbType.Int) { Value = filter.CityId },
                new SqlParameter("@districtId", SqlDbType.Int) { Value = filter.DistrictId },
                new SqlParameter("@pharmacySpecialtyId", SqlDbType.Int) { Value = filter.PharmacySpecialtyId },
                new SqlParameter("@userIsFoodAndDrug", SqlDbType.Bit) { Value = filter.UserIsFoodAndDrug },
                new SqlParameter("@pharmacyAccountID", SqlDbType.Int)
                {
                    Value = (object?)filter.PharmacyAccountId ?? DBNull.Value
                },
                new SqlParameter("@pageNumber", SqlDbType.Int) { Value = filter.PageNumber },
                new SqlParameter("@pageSize", SqlDbType.Int) { Value = filter.PageSize }
            };

            const string sql = @"
                EXEC [dbo].[sp_GetSimilarBrandPharmacyRequests_District_Speciality_Ranking10]
                    @brandIrc,
                    @provId,
                    @cityId,
                    @districtId,
                    @pharmacySpecialtyId,
                    @userIsFoodAndDrug,
                    @pharmacyAccountID,
                    @pageNumber,
                    @pageSize";

            return await context.Set<PatientReferralSimilarBrandPagedResult>()
                .FromSqlRaw(sql, parameters)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

    }
    
}
