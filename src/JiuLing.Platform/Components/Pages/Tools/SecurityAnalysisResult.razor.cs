using JiuLing.Platform.Components.Common;
using MudBlazor;

namespace JiuLing.Platform.Components.Pages.Tools;

public partial class SecurityAnalysisResult(
    IDialogService dialogService,
    IVirusTotalService virusTotalService,
    NavigationManager navigationManager)
{
    [Parameter]
    public string Hash { get; set; } = null!;

    private AnalysisResultDto? _analysisResult;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        var result = await virusTotalService.CheckByFileHashAsync(Hash);
        if (!result.Succeed)
        {
            await dialogService.ShowMessageBoxAsync(result.Message);
            navigationManager.NavigateTo($"/security");
            return;
        }

        _analysisResult = result.Dto;
    }

    private void GotoSecurityAnalysis()
    {
        navigationManager.NavigateTo($"/security");
    }
}