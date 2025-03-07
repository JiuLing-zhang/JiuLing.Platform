using JiuLing.Platform.Common.Enums;

namespace JiuLing.Platform.Models;
public class AppInfoDto
{
    /// <summary>
    /// 文件名
    /// </summary> 
    public string AppName { get; set; } = null!;
    public string Icon { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? GitHub { get; set; }
    public int DownloadCount { get; set; }
    public List<AppVersionInfoDto>? Versions { get; set; }
}

public class AppVersionInfoDto
{
    public PlatformEnum PlatformType { get; set; }
    public string VersionName { get; set; } = null!;
    public DateTime CreateTime { get; set; }
    public string PublishTime => CreateTime.ToString("yyyy-MM-dd");
    public string DownloadPath { get; set; } = null!;
    public SignTypeEnum SignType { get; set; }
    public string SignValue { get; set; } = null!;
    public int FileLength { get; set; }
    public string FileLengthMb => FileLength == 0 ? "未知" : $"{((double)FileLength / 1024 / 1024).ToString("0.00")} MB";
}