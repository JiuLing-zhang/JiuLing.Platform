namespace JiuLing.Platform.Services.HashCheckService;
public interface IHashService
{
    Task<string> ComputeHashAsync(string text, bool isToUpper);

    Task<string> ComputeHashAsync(byte[] buffer, bool isToUpper);
}