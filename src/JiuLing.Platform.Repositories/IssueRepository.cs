using JiuLing.CommonLibs.ExtensionMethods;
using JiuLing.Platform.Common;
using JiuLing.Platform.Models;

namespace JiuLing.Platform.Repositories;
public class IssueRepository(IDbContextFactory<AppDbContext> dbContextFactory) : IIssueRepository
{
    public async Task<PagedResult<IssueListDto>> GetIssuesAsync(PagedQuery query, string? appKey, IssueTypeEnum? type, IssueStatusEnum? status, string? searchKeyword)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var queryable = context.Issues.AsQueryable();

        if (appKey.IsNotEmpty())
        {
            queryable = queryable.Where(i => i.AppKey == appKey);
        }

        if (type != null)
        {
            queryable = queryable.Where(i => i.Type == type);
        }

        if (status != null)
        {
            queryable = queryable.Where(i => i.Status == status);
        }

        if (searchKeyword.IsNotEmpty())
        {
            queryable = queryable.Where(i => i.Title.Contains(searchKeyword));
        }

        var count = await queryable.CountAsync();
        var items = await queryable
            .Join(context.Users, issue => issue.UserId, user => user.Id, (issue, user) => new { issue, user })
            .Join(context.Apps, issueUser => issueUser.issue.AppKey, app => app.AppKey, (issueUser, app) => new IssueListDto
            {
                Id = issueUser.issue.Id,
                Title = issueUser.issue.Title,
                Type = issueUser.issue.Type,
                Status = issueUser.issue.Status,
                CreateTime = issueUser.issue.CreateTime,
                AppName = app.AppName,
                Username = issueUser.user.Username,
                AvatarUrl = issueUser.user.AvatarUrl
            })
            .OrderByDescending(i => i.Id)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new PagedResult<IssueListDto>()
        {
            TotalCount = count,
            Items = items
        };
    }

    public async Task<IssueDetailDto?> GetIssueByIdAsync(int id)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        return await context.Issues
            .Where(i => i.Id == id)
            .Join(context.Users, issue => issue.UserId, user => user.Id, (issue, user) => new { issue, user })
            .Join(context.Apps, issueUser => issueUser.issue.AppKey, app => app.AppKey, (issueUser, app) => new IssueDetailDto
            {
                Id = issueUser.issue.Id,
                Title = issueUser.issue.Title,
                Description = issueUser.issue.Description,
                Type = issueUser.issue.Type,
                ReproSteps = issueUser.issue.ReproSteps,
                OS = issueUser.issue.OS,
                AppVersion = issueUser.issue.AppVersion,
                AppName = app.AppName,
                UserId = issueUser.issue.UserId,
                Username = issueUser.user.Username,
                AvatarUrl = issueUser.user.AvatarUrl,
                Status = issueUser.issue.Status,
                CreateTime = issueUser.issue.CreateTime
            })
            .FirstOrDefaultAsync();
    }

    public async Task<Issue> AddIssueAsync(Issue issue)
    {
        issue.Title = issue.Title.Trim();
        await using var context = await dbContextFactory.CreateDbContextAsync();
        context.Issues.Add(issue);
        await context.SaveChangesAsync();
        return issue;
    }

    public async Task AddSubscribeAsync(int issueId, int userId)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var subscription = new IssueSubscription
        {
            IssueId = issueId,
            UserId = userId,
        };

        context.IssueSubscriptions.Add(subscription);
        await context.SaveChangesAsync();
    }

    public async Task RemoveSubscribeAsync(int issueId, int userId)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var subscription = await context.IssueSubscriptions.FirstOrDefaultAsync(x => x.IssueId == issueId && x.UserId == userId);
        if (subscription != null)
        {
            context.IssueSubscriptions.Remove(subscription);
            await context.SaveChangesAsync();
        }
    }

    public async Task UpdateIssueStatusAsync(int issueId, IssueStatusEnum status)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var issue = await context.Issues.FindAsync(issueId);
        if (issue != null)
        {
            issue.Status = status;
            await context.SaveChangesAsync();
        }
    }

    public async Task UpdateIssueTitleAsync(int issueId, string title)
    {
        title = title.Trim();
        await using var context = await dbContextFactory.CreateDbContextAsync();
        var issue = await context.Issues.FindAsync(issueId);
        if (issue != null)
        {
            issue.Title = title;
            await context.SaveChangesAsync();
        }
    }
    public async Task AddCommentAsync(IssueComment commentDto)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        await dbContext.IssueComments.AddAsync(commentDto);
        await dbContext.SaveChangesAsync();
    }
    public async Task<List<IssueCommentDto>> GetCommentsAsync(int issueId)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        return await context.IssueComments
            .Where(c => c.IssueId == issueId)
            .Join(context.Users, comment => comment.UserId, user => user.Id, (comment, user) => new IssueCommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                UserId = comment.UserId,
                Username = user.Username,
                AvatarUrl = user.AvatarUrl,
                IssueId = comment.IssueId,
                CreateTime = comment.CreateTime
            })
            .OrderBy(x => x.Id)
            .ToListAsync();
    }

    public async Task<List<IssueSubscriberDto>> GetSubscribersAsync(int issueId)
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();
        return await context.IssueSubscriptions
            .Where(s => s.IssueId == issueId)
            .Join(context.Users, subscription => subscription.UserId, user => user.Id, (subscription, user) => new IssueSubscriberDto
            {
                IssueId = subscription.IssueId,
                UserId = subscription.UserId,
                Username = user.Username,
                AvatarUrl = user.AvatarUrl,
                Email = user.Email
            })
            .ToListAsync();
    }
}