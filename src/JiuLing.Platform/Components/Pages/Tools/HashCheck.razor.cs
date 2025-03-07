using JiuLing.Platform.Components.Common;
using JiuLing.Platform.Services.HashCheckService;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace JiuLing.Platform.Components.Pages.Tools;

public partial class HashCheck(
    IJSRuntime jsRuntime,
    HashServiceFactory hashServiceFactory)
{
    private HashTypeEnum _currentHashType = HashTypeEnum.MD5;
    public HashTypeEnum CurrentHashType
    {
        get => _currentHashType;
        set
        {
            _currentHashType = value;
            Task.Run(GetHashInnerAsync);
        }
    }

    private HashInputTypeEnum _hashInputType = HashInputTypeEnum.Text;

    private bool _isLoading;

    private const string DefaultDragClass = "relative rounded-lg border-2 border-dashed pa-2 mt-2 mud-width-full mud-height-full";
    private string _dragClass = DefaultDragClass;

    /// <summary>
    /// 计算方式是否为字符串计算
    /// </summary>
    private bool _isCalculateForText = true;
    private string _inputText = "";
    private string _fileName = "";
    private byte[]? _fileBytes;
    private bool _isUpper;
    private string _hashResult = "";
    public string HashResult
    {
        get => _hashResult;
        set
        {
            _hashResult = value;
            _isCopySuccess = false;
        }
    }

    private bool _isCopySuccess;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await jsRuntime.InvokeVoidAsync("eval", @"
                var script1 = document.createElement('script');
                script1.src = '/js/copy-to-clipboard.js';
                document.head.appendChild(script1);

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
        _fileBytes = ms.ToArray();
        _isCalculateForText = false;
        await GetHashInnerAsync();
    }

    private void SetDragClass()
    {
        _dragClass = $"{DefaultDragClass} mud-border-primary";
    }

    private void ClearDragClass()
    {
        _dragClass = DefaultDragClass;
    }
    private async Task OnInputTextChanged(string value)
    {
        _isCalculateForText = true;
        _inputText = value;
        await GetHashInnerAsync();
    }

    private Task OnIsUpperChanged(bool value)
    {
        if (value)
        {
            HashResult = HashResult.ToUpper();
        }
        else
        {
            HashResult = HashResult.ToLower();
        }
        _isUpper = value;
        return Task.CompletedTask;
    }

    private async Task GetHashInnerAsync()
    {
        if ((_isCalculateForText && _inputText.IsEmpty()) || (!_isCalculateForText && _fileBytes == null))
        {
            HashResult = "";
            _isLoading = false;
            return;
        }

        var hashService = hashServiceFactory.Create(CurrentHashType);
        if (_isCalculateForText)
        {
            HashResult = await hashService.ComputeHashAsync(_inputText, _isUpper);
        }
        else
        {
            HashResult = await hashService.ComputeHashAsync(_fileBytes, _isUpper);
        }
        _isLoading = false;
        await InvokeAsync(StateHasChanged);
    }
    private async Task DoContentCopyAsync()
    {
        await jsRuntime.InvokeAsync<object>("copyToClipboard", HashResult);
        _isCopySuccess = true;
    }
}