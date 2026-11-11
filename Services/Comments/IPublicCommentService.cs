using darooyab_rewrite.ViewModels;

namespace darooyab_rewrite.Services.Comments;

public interface IPublicCommentService
{
    Task<IReadOnlyList<DrugComment>> GetApprovedCommentsAsync(
        string targetType,
        long targetId,
        CancellationToken cancellationToken);

    Task<bool> SubmitAsync(
        PublicCommentSubmission submission,
        CancellationToken cancellationToken);
}
