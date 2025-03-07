namespace JiuLing.Platform.Repositories;
public class AppReleaseRepository(IDbContextFactory<AppDbContext> dbContextFactory) : IAppReleaseRepository
{
    public async Task<AppRelease?> GetLastVersionAsync(string appKey, PlatformEnum platform)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.AppRelease.OrderByDescending(x => x.CreateTime).FirstOrDefaultAsync(x => x.AppKey == appKey && x.Platform == platform.ToString());
    }

    public async Task<int> AddAsync(AppRelease appInfo)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.AppRelease.Add(appInfo);
        return await dbContext.SaveChangesAsync();
    }

    public async Task<List<AppRelease>> GetAllAsync()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.AppRelease.Where(x => x.IsEnabled).ToListAsync();
    }
}