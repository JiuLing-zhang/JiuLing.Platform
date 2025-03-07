namespace JiuLing.Platform.Components.Pages;
public partial class Components(IAppService appService)
{
    private List<ComponentInfoDto>? _components;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await GetComponentsAsync();
    }

    private async Task GetComponentsAsync()
    {
        _components = await appService.GetComponentsAsync();
    }
}