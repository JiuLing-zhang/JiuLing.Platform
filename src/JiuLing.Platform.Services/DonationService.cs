namespace JiuLing.Platform.Services;

/// <summary>
/// 打赏服务
/// </summary>
public class DonationService(
    IDonationUsageRepository donationUsageRepository,
    IDonationRepository donationRepository
    ) : IDonationService
{

    /// <summary>
    /// 获取打赏用途列表
    /// </summary>
    public async Task<List<DonationUsageDto>> GetDonationUsagesAsync()
    {
        var donationUsages = await donationUsageRepository.GetAllAsync();
        return donationUsages.Select(x => new DonationUsageDto
        {
            Title = x.Title,
            Description = x.Description,
            Amount = x.Amount
        }).ToList();
    }

    /// <summary>
    /// 获取打赏列表
    /// </summary>
    public async Task<List<DonationDto>> GetDonationsAsync()
    {
        var components = await donationRepository.GetAllAsync();
        return components.Select(x => new DonationDto
        {
            Time = x.Time,
            User = x.User.MaskName(),
            Amount = x.Amount,
            Message = x.Message,
            Note = x.Note
        }).ToList();
    }
}