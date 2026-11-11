using darooyab.Models;
using darooyab_rewrite.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace darooyab_rewrite.Services.Comments;

internal sealed class PublicCommentService(darooConContext context) : IPublicCommentService
{
    public async Task<IReadOnlyList<DrugComment>> GetApprovedCommentsAsync(
        string targetType,
        long targetId,
        CancellationToken cancellationToken)
    {
        var query = context.Comments
            .AsNoTracking()
            .Where(comment => comment.IsAproved == true);

        query = targetType switch
        {
            "generic" when targetId <= int.MaxValue =>
                query.Where(comment => comment.DrugIdFk == (int)targetId),
            "brand" => query.Where(comment => comment.IrcFk == targetId),
            "doctor" when targetId <= int.MaxValue =>
                query.Where(comment => comment.CommentForWebSiteAccountUserFk == (int)targetId),
            _ => query.Where(_ => false)
        };

        var rows = await query
            .OrderByDescending(comment => comment.Id)
            .Take(100)
            .Select(comment => new
            {
                comment.Id,
                comment.ParentId,
                Name = comment.NameFamily ?? string.Empty,
                Content = comment.CommentText ?? string.Empty,
                comment.InsertDateTime
            })
            .ToListAsync(cancellationToken);

        var repliesByParent = rows
            .Where(comment => comment.ParentId.HasValue)
            .GroupBy(comment => comment.ParentId!.Value)
            .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<DrugCommentReply>)group
                    .OrderBy(comment => comment.Id)
                    .Select(comment => new DrugCommentReply(
                        string.IsNullOrWhiteSpace(comment.Name) ? "دارویاب" : comment.Name,
                        comment.Content,
                        comment.InsertDateTime))
                    .ToArray());

        return rows
            .Where(comment => !comment.ParentId.HasValue)
            .Take(20)
            .Select(comment => new DrugComment(
                string.IsNullOrWhiteSpace(comment.Name) ? "کاربر" : comment.Name,
                comment.Content,
                comment.InsertDateTime,
                repliesByParent.TryGetValue(comment.Id, out var replies) ? replies : []))
            .ToArray();
    }

    public async Task<bool> SubmitAsync(
        PublicCommentSubmission submission,
        CancellationToken cancellationToken)
    {
        var name = NormalizePersian(submission.Name.Trim());
        var content = NormalizePersian(submission.Content.Trim());
        var targetIsValid = submission.TargetType switch
        {
            "generic" when submission.TargetId <= int.MaxValue =>
                await context.DrugsNames
                    .AnyAsync(item => item.DrugId == (int)submission.TargetId, cancellationToken),
            "brand" =>
                await context.DrugsNames
                    .AnyAsync(item => item.Irc == submission.TargetId, cancellationToken),
            "doctor" when submission.TargetId <= int.MaxValue =>
                await context.WebSiteUserAccounts
                    .AnyAsync(item => item.Id == (int)submission.TargetId, cancellationToken),
            _ => false
        };

        if (!targetIsValid)
        {
            return false;
        }

        var comment = new Comment
        {
            IsAproved = false,
            CommentText = content,
            ParentId = null,
            InsertDateTime = DateTime.Now,
            NameFamily = name,
            Email = submission.Email?.Trim() ?? string.Empty,
            Tel = submission.Phone?.Trim() ?? string.Empty,
            IrcFk = submission.TargetType == "brand" ? submission.TargetId : null,
            DrugIdFk = submission.TargetType == "generic" ? (int)submission.TargetId : null,
            CommentForWebSiteAccountUserFk =
                submission.TargetType == "doctor" ? (int)submission.TargetId : null
        };

        context.Comments.Add(comment);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static string NormalizePersian(string value)
    {
        return value.Replace('ي', 'ی').Replace('ك', 'ک');
    }
}
