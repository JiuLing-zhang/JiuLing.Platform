namespace JiuLing.Platform.Repositories;

/// <summary>
/// 打赏用途仓储接口
/// </summary>
public interface IDonationUsageRepository
{
    public Task<List<DonationUsage>> GetAllAsync();
}