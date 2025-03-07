using JiuLing.CommonLibs;

namespace JiuLing.Platform.Services;
public class AppService(
    IAppReleaseRepository appReleaseRepository,
    IAppBaseRepository appBaseRepository,
    IComponentRepository componentRepository)
    : IAppService
{
    public async Task<List<AppDetailDto>> GetAppNamesAsync()
    {
        var appBaseList = await appBaseRepository.GetAllAsync();
        return appBaseList.Select(x => new AppDetailDto
        {
            AppKey = x.AppKey,
            AppName = x.AppName
        }).ToList();
    }

    public async Task<bool> AllowPublishAsync(string appKey, PlatformEnum platform, string versionName)
    {
        if (!await appBaseRepository.ExistAsync(appKey))
        {
            return false;
        }
        var release = await appReleaseRepository.GetLastVersionAsync(appKey, platform);
        if (release == null)
        {
            return true;
        }

        if (!VersionUtils.CheckNeedUpdate(release.VersionName, versionName))
        {
            return false;
        }
        return true;
    }

    public async Task<bool> PublishAsync(AppReleaseDto dto)
    {
        string minVersionName;
        if (dto.IsMinVersion)
        {
            //如果是最小版本，版本号直接取自身
            minVersionName = dto.VersionName;
        }
        else
        {
            var lastVersion = await appReleaseRepository.GetLastVersionAsync(dto.AppKey, dto.Platform);
            //第一次发布版本时，最小版本号取自身版本
            minVersionName = lastVersion == null ? dto.VersionName : lastVersion.MinVersionName;
        }

        var version = new Version(dto.VersionName);
        var appRelease = new AppRelease()
        {
            AppKey = dto.AppKey,
            Platform = dto.Platform.ToString(),
            VersionCode = version.Revision,
            VersionName = dto.VersionName,
            MinVersionName = minVersionName,
            UpgradeLog = dto.UpgradeLog ?? "",
            FilePath = dto.FilePath,
            SignType = dto.SignType.ToString(),
            SignValue = dto.SignValue,
            IsEnabled = true,
            FileLength = dto.FileLength,
            CreateTime = DateTime.Now
        };

        var count = await appReleaseRepository.AddAsync(appRelease);
        return count > 0;
    }
    public async Task<List<AppInfoDto>> GetAppsAsync()
    {
        var result = new List<AppInfoDto>();

        var appBaseList = await appBaseRepository.GetAllAsync();
        var appInfoList = await appReleaseRepository.GetAllAsync();
        foreach (var appBase in appBaseList)
        {
            var resultItem = new AppInfoDto()
            {
                AppName = appBase.AppName,
                Icon = appBase.Icon,
                Description = appBase.Description,
                DownloadCount = appBase.DownloadCount,
                GitHub = appBase.GitHub
            };

            if (!appBase.IsShow)
            {
                result.Add(resultItem);
                continue;
            }

            string appKey = appBase.AppKey;

            IEnumerable<AppRelease> appVersions = appInfoList.Where(x => x.AppKey == appKey).ToList();

            List<AppVersionInfoDto> versions = new List<AppVersionInfoDto>();
            BuildAppInfo(appVersions, PlatformEnum.Windows, out var windowsVersion);
            if (windowsVersion != null)
            {
                versions.Add(windowsVersion);
            }

            BuildAppInfo(appVersions, PlatformEnum.Android, out var androidVersion);
            if (androidVersion != null)
            {
                versions.Add(androidVersion);
            }

            BuildAppInfo(appVersions, PlatformEnum.iOS, out var iosVersion);
            if (iosVersion != null)
            {
                versions.Add(iosVersion);
            }

            if (versions.Count > 0)
            {
                resultItem.Versions = versions;
            }

            result.Add(resultItem);
        }

        return result;
    }

    private void BuildAppInfo(IEnumerable<AppRelease> apps, PlatformEnum platform, out AppVersionInfoDto? versions)
    {
        var platformInfo = apps.Where(x => x.Platform == platform.ToString()).MaxBy(x => x.CreateTime);
        if (platformInfo == null)
        {
            versions = null;
            return;
        }

        versions = new AppVersionInfoDto()
        {
            CreateTime = platformInfo.CreateTime,
            DownloadPath = platformInfo.FilePath,
            PlatformType = platform,
            VersionName = platformInfo.VersionName,
            SignType = (SignTypeEnum)Enum.Parse(typeof(SignTypeEnum), platformInfo.SignType),
            SignValue = platformInfo.SignValue,
            FileLength = platformInfo.FileLength,
        };
    }

    public async Task<List<ComponentInfoDto>> GetComponentsAsync()
    {
        var components = await componentRepository.GetAllAsync();
        return components.Select(x => new ComponentInfoDto
        {
            Name = x.Name,
            Icon = x.Icon,
            Description = x.Description,
            GitHub = x.GitHub,
        }).ToList();
    }

    public async Task<string> GetAppKeyFromCheckUpdateKeyAsync(string checkUpdateKey)
    {
        return await appBaseRepository.GetAppKeyFromCheckUpdateKeyAsync(checkUpdateKey);
    }

    public async Task<AppReleaseDto?> GetAppReleaseInfoAsync(string appKey, PlatformEnum platform)
    {
        var appRelease = await appReleaseRepository.GetLastVersionAsync(appKey, platform);
        if (appRelease == null)
        {
            return null;
        }
        return new AppReleaseDto()
        {
            AppKey = appKey,
            CreateTime = appRelease.CreateTime,
            FileLength = appRelease.FileLength,
            FilePath = appRelease.FilePath,
            MinVersionName = appRelease.MinVersionName,
            Platform = Enum.Parse<PlatformEnum>(appRelease.Platform),
            SignType = Enum.Parse<SignTypeEnum>(appRelease.SignType),
            SignValue = appRelease.SignValue,
            UpgradeLog = appRelease.UpgradeLog,
            VersionCode = appRelease.VersionCode,
            VersionName = appRelease.VersionName
        };
    }

    public async Task DownloadOnceAsync(string appKey)
    {
        await appBaseRepository.DownloadOnceAsync(appKey);
    }
}