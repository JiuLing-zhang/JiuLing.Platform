using JiuLing.Platform.Common;
using MudBlazor;

namespace JiuLing.Platform.Components.Pages.Issue;

public partial class Issues(
    IAppService appService,
    IIssueService issueService,
    NavigationManager navigation
    )
{
    private List<AppDetailDto>? _apps;
    private MudTable<IssueListDto> _table = null!;
    private PagedResult<IssueListDto> _pagedResult = new();

    private readonly PagedQuery _pagedQuery = new();
    private string? _searchKeyword;
    private string? _appKey;

    private IssueStatusEnum? _status;

    private bool _issueLoading;

    protected override async Task OnInitializedAsync()
    {
        if (RendererInfo.IsInteractive)
        {
            _apps = await appService.GetAppNamesAsync();
            await LoadIssuesAsync();
        }
    }

    private async Task LoadIssuesAsync()
    {
        _issueLoading = true;
        await InvokeAsync(StateHasChanged);
        _pagedResult = await issueService.GetIssuesAsync(_pagedQuery, _appKey, null, _status, _searchKeyword);
        _issueLoading = false;
    }
    private async Task OnSearchAppKey(string input)
    {
        _appKey = input == "-1" ? null : input;
        _pagedQuery.PageIndex = 1;
        await LoadIssuesAsync();
    }
    private async Task OnSearchStatus(string input)
    {
        if (input == "-1")
        {
            _status = null;
        }
        else
        {
            _status = Enum.Parse<IssueStatusEnum>(input);
        }
        _pagedQuery.PageIndex = 1;
        await LoadIssuesAsync();
    }
    private async Task OnSearchKeyword(string input)
    {
        _searchKeyword = input;
        _pagedQuery.PageIndex = 1;
        await LoadIssuesAsync();
    }

    private async Task OnPageChanged(int page)
    {
        _pagedQuery.PageIndex = page;
        await LoadIssuesAsync();
    }

    private void ViewDetails(int id)
    {
        navigation.NavigateTo($"/issues/detail/{id}");
    }
}