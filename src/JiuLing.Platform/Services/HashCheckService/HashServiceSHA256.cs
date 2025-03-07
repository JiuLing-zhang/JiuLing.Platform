using Microsoft.JSInterop;

namespace JiuLing.Platform.Services.HashCheckService;
public class HashServiceSHA256(IJSRuntime jSRuntime) : IHashService
{
    public async Task<string> ComputeHashAsync(string text, bool isToUpper)
    {
        var value = await jSRuntime.InvokeAsync<string>("GetTextSHA256", text);
        return value.ToUpperOrLower(isToUpper);
    }

    public async Task<string> ComputeHashAsync(byte[] buffer, bool isToUpper)
    {
        var value = await jSRuntime.InvokeAsync<string>("GetFileSHA256", buffer);
        return value.ToUpperOrLower(isToUpper);
    }
}