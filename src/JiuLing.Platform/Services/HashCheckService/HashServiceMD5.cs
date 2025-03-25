using Microsoft.JSInterop;

namespace JiuLing.Platform.Services.HashCheckService;
public class HashServiceMD5(IJSRuntime jSRuntime) : IHashService
{
    public async Task<string> ComputeHashAsync(string text, bool isToUpper)
    {
        var value = await jSRuntime.InvokeAsync<string>("GetTextMD5", text);
        return value.ToUpperOrLower(isToUpper);
    }

    public async Task<string> ComputeHashAsync(byte[] buffer, bool isToUpper)
    {
        var value = await jSRuntime.InvokeAsync<string>("GetFileMD5", buffer);
        return value.ToUpperOrLower(isToUpper);
    }
}
