using JiuLing.Platform.Components.Common;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace JiuLing.Platform.Components.Pages.Issue;

public partial class IssueDetail(
    IIssueService issueService,
    AuthenticationStateProvider authenticationStateProvider,
    NavigationManager navigation,
    IDialogService dialog
    )
{
    [Parameter]
    public int Id { get; set; }

    private UserDto? _user;

    private IssueDetailDto? _issue;
    private List<IssueCommentDto> _comments = [];
    private List<IssueSubscriberDto> _subscribers = [];
    private string _comment = string.Empty;
    private bool _commentError;
    private string _commentErrorText = string.Empty;

    private bool _isSubmitting;
    protected override async Task OnInitializedAsync()
    {
        if (RendererInfo.IsInteractive)
        {
            _issue = await issueService.GetIssueByIdAsync(Id);
            if (_issue == null)
            {
                navigation.NavigateTo("/404");
                return;
            }
            var authState = await ((CustomAuthenticationStateProvider)authenticationStateProvider).GetAuthenticationStateAsync();
            _user = authState.UserInfo();

            _comments = await issueService.GetCommentsAsync(Id);
            _subscribers = await issueService.GetSubscribersAsync(Id);
        }
        await base.OnInitializedAsync();
    }

    private async Task AddComment()
    {
        if (_user == null)
        {
            _commentError = true;
            _commentErrorText = "请先登录";
            return;
        }

        if (_comment.IsEmpty())
        {
            _commentError = true;
            _commentErrorText = "评论内容不能为空";
            return;
        }

        if (_comment.Length < 2)
        {
            _commentError = true;
            _commentErrorText = "评论内容太短";
            return;
        }

        _isSubmitting = true;
        await InvokeAsync(StateHasChanged);
        var commentDto = new IssueCommentDto
        {
            IssueId = Id,
            UserId = _user.Id,
            Content = _comment,
            CreateTime = DateTime.Now
        };
        await issueService.AddCommentAsync(commentDto);
        _comments = await issueService.GetCommentsAsync(Id);
        _comment = string.Empty;
        _commentError = false;
        _commentErrorText = string.Empty;
        _isSubmitting = false;
    }

    private async Task ToggleSubscription()
    {
        if (_user == null)
        {
            return;
        }

        bool subscribed = _subscribers.All(x => x.UserId != _user.Id);
        await issueService.ToggleSubscriptionAsync(Id, _user.Id, subscribed);
        _subscribers = await issueService.GetSubscribersAsync(Id);
    }

    private async Task UpdateIssueTitle()
    {
        var result = await dialog.ShowYesOrNoAsync("确定要修改标题吗？", "确定");
        if (result is null or false)
        {
            return;
        }
        ArgumentNullException.ThrowIfNull(_issue, "Issue 没找到，为什么呢？");

        await issueService.UpdateIssueTitleAsync(_issue.Id, _issue.Title);
        await dialog.ShowInfoAsync("标题已更新");
    }

    private async Task CloseIssue()
    {
        var result = await dialog.ShowYesOrNoAsync("确定要关闭问题吗？", "确定");
        if (result is null or false)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(_issue, "Issue 没找到，为什么呢？");
        await issueService.UpdateIssueStatusAsync(_issue.Id, IssueStatusEnum.Close);
        await dialog.ShowInfoAsync("标题已更新");
        navigation.NavigateTo(navigation.Uri, forceLoad: true);
    }


    private void GoBack()
    {
        navigation.NavigateTo("/issues");
    }
}