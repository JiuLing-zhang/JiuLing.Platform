using JiuLing.Platform.Models.Entities;

namespace JiuLing.Platform.Repositories;
public class AppReleaseRepository(IDbContextFactory<AppDbContext> dbContextFactory) : IAppReleaseRepository
{
    public async Task<AppRelease?> GetLastVersionAsync(string appKey, PlatformEnum platform)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.AppReleases.OrderByDescending(x => x.CreateTime).FirstOrDefaultAsync(x => x.AppKey == appKey && x.Platform == platform.ToString());
    }

    public async Task<int> AddAsync(AppRelease appInfo)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.AppReleases.Add(appInfo);
        return await dbContext.SaveChangesAsync();
    }

    public async Task<List<AppRelease>> GetAllAsync()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.AppReleases.Where(x => x.IsEnabled).ToListAsync();
    }

    public async Task<List<AppRelease>> GetAppReleaseAsync(string appKey)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.AppReleases.Where(x => x.IsEnabled && x.AppKey == appKey).OrderByDescending(x => x.CreateTime).ToListAsync();
    }
}