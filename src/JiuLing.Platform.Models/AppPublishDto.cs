using JiuLing.Platform.Common.Enums;

namespace JiuLing.Platform.Models;
public class AppPublishDto
{
    public string AppKey { get; set; } = null!;
    public PlatformEnum Platform { get; set; } = PlatformEnum.Windows;
    public string VersionName { get; set; } = null!;
    public bool IsMinVersion { get; set; } = false;
    public string? Log { get; set; }
    public SignTypeEnum SignType { get; set; } = SignTypeEnum.SHA1;
}