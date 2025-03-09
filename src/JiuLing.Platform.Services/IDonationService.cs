namespace JiuLing.Platform.Services;
public interface IDonationService
{
    Task<List<DonationDto>> GetDonationsAsync();
}