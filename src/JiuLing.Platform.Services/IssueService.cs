using JiuLing.Platform.Common;

namespace JiuLing.Platform.Services;
public class IssueService(
    IIssueRepository issueRepository,
    IUserRepository userRepository,
    IAppBaseRepository appBaseRepository,
    EmailService emailService
    ) : IIssueService
{
    public async Task<PagedResult<IssueListDto>> GetIssuesAsync(PagedQuery query, string? appKey, IssueTypeEnum? type, IssueStatusEnum? status, string? searchKeyword)
    {
        return await issueRepository.GetIssuesAsync(query, appKey, type, status, searchKeyword);
    }

    public async Task<IssueDetailDto?> GetIssueByIdAsync(int id)
    {
        return await issueRepository.GetIssueByIdAsync(id);
    }
    public async Task CreateIssueAsync(IssueDto issueDto)
    {
        var adminUsers = await userRepository.GetAdminUsersAsync();
        var app = await appBaseRepository.GetOneAsync(issueDto.AppKey);
        ArgumentNullException.ThrowIfNull(app, "App 没找到，为什么呢？");
        var issue = new Issue
        {
            Id = issueDto.Id,
            Title = issueDto.Title,
            Description = issueDto.Description,
            Type = issueDto.Type,
            ReproSteps = issueDto.ReproSteps ?? "",
            OS = issueDto.OS ?? "",
            AppVersion = issueDto.AppVersion ?? "",
            Status = IssueStatusEnum.Open,
            AppKey = issueDto.AppKey,
            UserId = issueDto.UserId,
            CreateTime = issueDto.CreateTime
        };
        var entity = await issueRepository.AddIssueAsync(issue);

        if (issueDto.SubscribeToUpdates)
        {
            await issueRepository.AddSubscribeAsync(entity.Id, issueDto.UserId);
        }

        // 管理员默认订阅和接收邮件
        foreach (var adminUser in adminUsers)
        {
            await issueRepository.AddSubscribeAsync(entity.Id, adminUser.Id);
            await emailService.SendIssueCreateEmailAsync(adminUser.Email, app.AppName, issueDto.Title, issueDto.Description);
        }
    }

    public async Task UpdateIssueStatusAsync(int issueId, IssueStatusEnum status)
    {
        var issueDetailDto = await issueRepository.GetIssueByIdAsync(issueId);
        ArgumentNullException.ThrowIfNull(issueDetailDto, "Issue 没找到，为什么呢？");
        var users = await userRepository.GetUsersAsync();
        var subscribers = await issueRepository.GetSubscribersAsync(issueId);
        foreach (var subscriber in subscribers)
        {
            var user = users.First(x => x.Id == subscriber.UserId);
            if (user.Role == UserRoleEnum.Admin)
            {
                // 只有管理员才能关闭，所以节约资源不需要发送邮件
                continue;
            }
            await emailService.SendIssueStatusChangeEmailAsync(user.Email, issueDetailDto.AppName, issueDetailDto.Title, status);
        }

        await issueRepository.UpdateIssueStatusAsync(issueId, status);
    }

    public async Task UpdateIssueTitleAsync(int issueId, string title)
    {
        var issueDetailDto = await issueRepository.GetIssueByIdAsync(issueId);
        ArgumentNullException.ThrowIfNull(issueDetailDto, "Issue 没找到，为什么呢？");

        var users = await userRepository.GetUsersAsync();
        var subscribers = await issueRepository.GetSubscribersAsync(issueId);
        foreach (var subscriber in subscribers)
        {
            var user = users.First(x => x.Id == subscriber.UserId);
            if (user.Role == UserRoleEnum.Admin)
            {
                // 只有管理员才能修改标题，所以节约资源不需要发送邮件
                continue;
            }
            await emailService.SendIssueTitleChangeEmailAsync(user.Email, issueDetailDto.AppName, issueDetailDto.Title, title);
        }
        await issueRepository.UpdateIssueTitleAsync(issueId, title);
    }

    public async Task<List<IssueCommentDto>> GetCommentsAsync(int issueId)
    {
        return await issueRepository.GetCommentsAsync(issueId);
    }

    public async Task<List<IssueSubscriberDto>> GetSubscribersAsync(int issueId)
    {
        return await issueRepository.GetSubscribersAsync(issueId);
    }

    public async Task AddCommentAsync(IssueCommentDto commentDto)
    {
        var issueDetailDto = await issueRepository.GetIssueByIdAsync(commentDto.IssueId);
        ArgumentNullException.ThrowIfNull(issueDetailDto, "Issue 没找到，为什么呢？");

        var users = await userRepository.GetUsersAsync();
        var subscribers = await issueRepository.GetSubscribersAsync(commentDto.IssueId);
        foreach (var subscriber in subscribers)
        {
            if (subscriber.UserId == commentDto.UserId)
            {
                // 节约资源，不需要给自己发邮件
                continue;
            }
            var user = users.First(x => x.Id == subscriber.UserId);
            await emailService.SendIssueCommentEmailAsync(user.Email, issueDetailDto.AppName, issueDetailDto.Title, commentDto.Content);
        }

        await issueRepository.AddCommentAsync(new IssueComment
        {
            IssueId = commentDto.IssueId,
            Content = commentDto.Content,
            UserId = commentDto.UserId,
            CreateTime = commentDto.CreateTime
        });
    }

    public async Task ToggleSubscriptionAsync(int issueId, int userId, bool subscribed)
    {
        if (subscribed)
        {
            await issueRepository.AddSubscribeAsync(issueId, userId);
        }
        else
        {
            await issueRepository.RemoveSubscribeAsync(issueId, userId);
        }
    }
}