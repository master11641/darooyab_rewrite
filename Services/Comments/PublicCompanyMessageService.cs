using darooyab.Models;
using darooyab_rewrite.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace darooyab_rewrite.Services.Comments;

internal sealed class PublicCompanyMessageService(darooConContext context)
    : IPublicCompanyMessageService
{
    public async Task<bool> SubmitAsync(
        CompanyMessageSubmission submission,
        string? remoteIpAddress,
        CancellationToken cancellationToken)
    {
        var companyName = await context.Factorys
            .AsNoTracking()
            .Where(factory =>
                factory.FactoryId == submission.CompanyId &&
                factory.CountryId == 1)
            .Select(factory => factory.FactoryName)
            .SingleOrDefaultAsync(cancellationToken);
        if (companyName is null)
        {
            return false;
        }

        if (remoteIpAddress?.Length > 25)
        {
            remoteIpAddress = null;
        }

        context.TblContactus.Add(new TblContactu
        {
            TblContactusNameAndFamily = submission.Name?.Trim() ?? string.Empty,
            TblContactusCategoryIdFk = 53,
            TblContactusOrganizationOrCompanyName = string.Empty,
            TblContactusRole = $"پیام کاربر به {companyName}",
            TblContactusTel = submission.Phone?.Trim() ?? string.Empty,
            TblContactusEmail = string.Empty,
            TblContactusTextMessage = submission.Content.Trim(),
            TblContactusViewByManagerStatusNum = 2,
            TblContactusIpsystem = remoteIpAddress ?? string.Empty,
            TblContactusRegisterDate = DateTime.Now.Date,
            TblContactusViewByManagerDate = null
        });

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
