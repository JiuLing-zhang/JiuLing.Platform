namespace JiuLing.Platform.Repositories;

/// <summary>
/// 打赏用途仓储接口
/// </summary>
public class DonationUsageRepository(IDbContextFactory<AppDbContext> dbContextFactory) : IDonationUsageRepository
{
    public async Task<List<DonationUsage>> GetAllAsync()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.DonationUsages.OrderBy(x => x.Id).ToListAsync();
    }
}