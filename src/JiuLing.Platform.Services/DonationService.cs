namespace JiuLing.Platform.Services;
public class DonationService(IDonationRepository donationRepository) : IDonationService
{
    public async Task<List<DonationDto>> GetDonationsAsync()
    {
        var components = await donationRepository.GetAllAsync();
        return components.Select(x => new DonationDto
        {
            Time = x.Time,
            User = x.User,
            Amount = x.Amount,
            Message = x.Message,
            Note = x.Note
        }).ToList();
    }
}