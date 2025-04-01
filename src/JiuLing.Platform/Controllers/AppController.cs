using JiuLing.CommonLibs.Model;
using Microsoft.AspNetCore.Mvc;

namespace JiuLing.Platform.Controllers;

[Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
[ApiController]
public class AppController(IAppService appService) : ControllerBase
{
    [HttpGet("check-update/{key}/{platform}")]
    public async Task<IActionResult> CheckUpdateAsync(string key, PlatformEnum platform)
    {
        var appKey = await appService.GetAppKeyFromCheckUpdateKeyAsync(key);
        if (appKey.IsEmpty())
        {
            return Ok(new ApiResponse(1, "无可用更新"));
        }
        var appRelease = await appService.GetAppReleaseInfoAsync(appKey, platform);
        if (appRelease == null)
        {
            return Ok(new ApiResponse(1, "无可用更新"));
        }

        var scheme = this.Request.Headers["X-Forwarded-Proto"].ToString();
        if (scheme.IsEmpty())
        {
            scheme = this.Request.Scheme;
        }
        string downloadUrl = $"{scheme}://{this.Request.Host}{this.Request.PathBase}{appRelease.FilePath}";
        var upgradeInfo = new AppUpdateInfo
        {
            Name = appRelease.AppKey,
            VersionCode = appRelease.VersionCode,
            Version = appRelease.VersionName,
            MinVersion = appRelease.MinVersionName,
            FileLength = appRelease.FileLength,
            DownloadUrl = downloadUrl,
            CreateTime = appRelease.CreateTime,
            Log = appRelease.UpgradeLog ?? "",
            SignType = appRelease.SignType.ToString(),
            SignValue = appRelease.SignValue ?? ""
        };

        return Ok(upgradeInfo);
    }
}
