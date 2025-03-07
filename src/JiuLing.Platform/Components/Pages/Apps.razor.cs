namespace JiuLing.Platform.Components.Pages;

public partial class Apps(IAppService appService)
{

    private List<AppInfoDto>? _apps;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await GetAppsAsync();
    }

    private async Task GetAppsAsync()
    {
        _apps = await appService.GetAppsAsync();
    }
}