using JiuLing.Platform.Components.Common;
using MudBlazor;

namespace JiuLing.Platform.Components.Pages.Tools;

public partial class SecurityAnalysisResult(
    IDialogService dialogService,
    IVirusTotalService virusTotalService,
    NavigationManager navigation)
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
            navigation.NavigateTo($"/security");
            return;
        }

        _analysisResult = result.Dto;
    }

    private void GotoSecurityAnalysis()
    {
        navigation.NavigateTo($"/security");
    }
}