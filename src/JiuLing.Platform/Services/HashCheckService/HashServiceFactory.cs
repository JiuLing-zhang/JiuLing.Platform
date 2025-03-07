using Microsoft.JSInterop;

namespace JiuLing.Platform.Services.HashCheckService;
public class HashServiceFactory(IJSRuntime jsRuntime)
{
    public IHashService Create(HashTypeEnum hashType)
    {
        return hashType switch
        {
            HashTypeEnum.MD5 => new HashServiceMD5(jsRuntime),
            HashTypeEnum.SHA1 => new HashServiceSHA1(jsRuntime),
            HashTypeEnum.SHA256 => new HashServiceSHA256(jsRuntime),
            _ => throw new Exception("无效的哈希类型"),
        };
    }
}