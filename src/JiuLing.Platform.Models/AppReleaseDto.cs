using JiuLing.Platform.Common.Enums;

namespace JiuLing.Platform.Models;

public class AppReleaseDto
{
    public DateTime CreateTime { get; set; }
    public string AppKey { get; set; } = null!;
    public PlatformEnum Platform { get; set; }
    public int VersionCode { get; set; }
    public string VersionName { get; set; } = null!;
    public string MinVersionName { get; set; } = null!;
    public bool IsMinVersion { get; set; }
    public string? UpgradeLog { get; set; }
    public string FilePath { get; set; } = null!;
    public int FileLength { get; set; }
    public SignTypeEnum SignType { get; set; }
    public string SignValue { get; set; } = null!;


}