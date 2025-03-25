using JiuLing.Platform.Components.Common;
using JiuLing.Platform.Services.HashCheckService;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;

namespace JiuLing.Platform.Components.Pages.Tools;

public partial class SecurityAnalysis(IJSRuntime jsRuntime,
    HashServiceFactory hashServiceFactory,
    NavigationManager navigation)
{
    private const string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-2 mt-2 mud-width-full mud-height-full";
    private string _dragClass = DefaultDragClass;

    private string _hash = "";
    private string _fileName = "";
    private bool _isLoading = false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await jsRuntime.InvokeVoidAsync("eval", @"
                var script2 = document.createElement('script');
                script2.src = '/js/crypto-js.js';
                document.head.appendChild(script2);

                var script3 = document.createElement('script');
                script3.src = '/js/crypto-js-inner.js';
                document.head.appendChild(script3);
            ");
        }
    }

    private async void OnInputFileChanged(InputFileChangeEventArgs e)
    {
        ClearDragClass();

        _isLoading = true;
        _fileName = e.GetMultipleFiles()[0].Name;
        var ms = new MemoryStream();
        await e.GetMultipleFiles()[0].OpenReadStream(long.MaxValue).CopyToAsync(ms);
        var fileBytes = ms.ToArray();
        var hashService = hashServiceFactory.Create(HashTypeEnum.SHA1);
        _hash = await hashService.ComputeHashAsync(fileBytes, false);
        GotoResultPage();
    }

    private void SetDragClass()
    {
        _dragClass = $"{DefaultDragClass} mud-border-primary";
    }

    private void ClearDragClass()
    {
        _dragClass = DefaultDragClass;
    }

    private void KeyUp(string key)
    {
        if (key != "Enter")
        {
            return;
        }
        GotoResultPage();
    }

    private void GotoResultPage()
    {
        if (_hash.IsEmpty())
        {
            return;
        }
        navigation.NavigateTo($"/security/{_hash}");
    }
}