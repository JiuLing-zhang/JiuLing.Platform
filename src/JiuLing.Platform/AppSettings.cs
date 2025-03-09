namespace JiuLing.Platform;


public class AppSettings
{
    public string VirusTotalApiKey { get; set; } = null!;
    public string StunServer { get; set; } = null!;

    public List<DonationTarget> DonationTargets { get; set; } = null!;

    /// <summary>
    /// 备案号
    /// </summary>
    public string BeiAn { get; set; } = null!;
}

/// <summary>
/// 捐赠目标
/// </summary>
public class DonationTarget
{
    public string Service { get; set; } = null!;
    public decimal Amount { get; set; }
}