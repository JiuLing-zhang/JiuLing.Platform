namespace JiuLing.Platform.Services;
public interface IAppService
{
    Task<List<AppDetailDto>> GetAppNamesAsync();
    Task<bool> AllowPublishAsync(string appKey, PlatformEnum platform, string versionName);
    Task<bool> PublishAsync(AppReleaseDto dto);
    Task<List<AppInfoDto>> GetAppsAsync();
    Task<string> GetAppKeyFromCheckUpdateKeyAsync(string checkUpdateKey);
    Task<AppReleaseDto?> GetAppReleaseInfoAsync(string appKey, PlatformEnum platform);
    Task DownloadOnceAsync(string appKey);
}