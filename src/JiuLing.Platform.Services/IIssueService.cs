using JiuLing.Platform.Common;

namespace JiuLing.Platform.Services;
public interface IIssueService
{
    Task<PagedResult<IssueListDto>> GetIssuesAsync(PagedQuery query, string? appKey, IssueTypeEnum? type, IssueStatusEnum? status, string? searchKeyword);

    Task<IssueDetailDto?> GetIssueByIdAsync(int id);

    Task CreateIssueAsync(IssueDto issueDto);

    Task UpdateIssueStatusAsync(int issueId, IssueStatusEnum status);

    Task UpdateIssueTitleAsync(int issueId, string title);

    Task<List<IssueCommentDto>> GetCommentsAsync(int issueId);

    Task<List<IssueSubscriberDto>> GetSubscribersAsync(int issueId);

    Task AddCommentAsync(IssueCommentDto commentDto);

    Task ToggleSubscriptionAsync(int issueId, int userId, bool subscribed);
}