using JiuLing.Platform.Components.Common;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using MudBlazor;

namespace JiuLing.Platform.Components.Pages.Issue;

public partial class NewIssue(
    IAppService appService,
    IIssueService issueService,
    IUserService userService,
    NavigationManager navigation,
    AuthenticationStateProvider authenticationStateProvider,
    IDialogService dialog
    )
{
    [Parameter]
    public string? TypeString { get; set; }
    [Parameter]
    [SupplyParameterFromQuery(Name = "app")]
    public string? AppKey { get; set; }

    private UserDto? _user;
    private MudForm _form = null!;
    private readonly IssueDto _model = new();
    private List<AppDetailDto>? _apps;
    private bool _isSubmitting;
    protected override async Task OnInitializedAsync()
    {
        if (RendererInfo.IsInteractive)
        {
            var authState = await ((CustomAuthenticationStateProvider)authenticationStateProvider).GetAuthenticationStateAsync();
            _user = authState.UserInfo();

            if (_user == null)
            {
                navigation.NavigateTo("/u/login", true);
                return;
            }

            if (!Enum.TryParse<IssueTypeEnum>(TypeString, out var type))
            {
                navigation.NavigateTo("/404");
                return;
            }

            _apps = await appService.GetAppNamesAsync();

            if (AppKey.IsNotEmpty() && _apps.All(x => x.AppKey != AppKey))
            {
                navigation.NavigateTo("/404");
                return;
            }

            _model.AppKey = AppKey.IsEmpty() ? "-1" : AppKey;
            _model.Type = type;
            _model.UserId = _user.Id;
            _model.SubscribeToUpdates = true;
        }
        await base.OnInitializedAsync();
    }

    private async Task OnSubmit()
    {
        await _form.Validate();
        if (!_form.IsValid)
        {
            return;
        }

        if (_model.AppKey == "-1")
        {
            await dialog.ShowInfoAsync("请选择应用");
            return;
        }

        _model.CreateTime = DateTime.Now;
        _isSubmitting = true;
        await InvokeAsync(StateHasChanged);
        await issueService.CreateIssueAsync(_model);
        navigation.NavigateTo("/issues");
    }
}