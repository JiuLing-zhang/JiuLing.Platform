namespace JiuLing.Platform.Components.Pages.ComputerLock;

public partial class Releases(IAppService appService)
{
    private List<AppReleaseDto>? _releases;
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _releases = await appService.GetReleaseAsync("computer-lock");
    }
}
