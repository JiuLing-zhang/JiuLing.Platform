namespace JiuLing.Platform.Services;

/// <summary>
/// 打赏服务接口
/// </summary>
public interface IDonationService
{
    /// <summary>
    /// 获取打赏用途列表
    /// </summary>
    Task<List<DonationUsageDto>> GetDonationUsagesAsync();

    /// <summary>
    /// 获取打赏列表
    /// </summary>
    Task<List<DonationDto>> GetDonationsAsync();
}