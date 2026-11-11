using darooyab_rewrite.ViewModels;

namespace darooyab_rewrite.Services.Comments;

public interface IPublicCompanyMessageService
{
    Task<bool> SubmitAsync(
        CompanyMessageSubmission submission,
        string? remoteIpAddress,
        CancellationToken cancellationToken);
}
