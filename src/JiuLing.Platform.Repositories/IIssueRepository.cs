using JiuLing.Platform.Common;
using JiuLing.Platform.Models;

namespace JiuLing.Platform.Repositories;

public interface IIssueRepository
{
    Task<PagedResult<IssueListDto>> GetIssuesAsync(PagedQuery query, string? appKey, IssueTypeEnum? type, IssueStatusEnum? status, string? searchKeyword);
    Task<IssueDetailDto?> GetIssueByIdAsync(int id);
    Task<Issue> AddIssueAsync(Issue issue);
    Task AddSubscribeAsync(int issueId, int userId);
    Task RemoveSubscribeAsync(int issueId, int userId);
    Task UpdateIssueStatusAsync(int issueId, IssueStatusEnum status);
    Task UpdateIssueTitleAsync(int issueId, string title);
    Task AddCommentAsync(IssueComment commentDto);
    Task<List<IssueCommentDto>> GetCommentsAsync(int issueId);
    Task<List<IssueSubscriberDto>> GetSubscribersAsync(int issueId);
}