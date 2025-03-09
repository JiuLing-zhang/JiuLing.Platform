namespace JiuLing.Platform.Repositories;
public class DonationRepository(IDbContextFactory<AppDbContext> dbContextFactory) : IDonationRepository
{
    public async Task<List<Donation>> GetAllAsync()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.Donations.OrderByDescending(x => x.Time).ToListAsync();
    }
}