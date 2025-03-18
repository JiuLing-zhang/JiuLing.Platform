using JiuLing.Platform.Models.Entities;

namespace JiuLing.Platform.Repositories;
public class AppBaseRepository(IDbContextFactory<AppDbContext> dbContextFactory) : IAppBaseRepository
{
    public async Task<List<App>> GetAllAsync()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.Apps.OrderBy(x => x.Sort).ToListAsync();
    }

    public async Task<App?> GetOneAsync(string appKey)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.Apps.FirstOrDefaultAsync(x => x.AppKey == appKey && x.IsShow);
    }

    public async Task<int> AddAsync(App appBase)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.Apps.Add(appBase);
        return await dbContext.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(int id)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var appBase = await dbContext.Apps.FindAsync(id);
        if (appBase == null)
        {
            return 0;
        }
        dbContext.Apps.Remove(appBase);
        return await dbContext.SaveChangesAsync();
    }

    public async Task DownloadOnceAsync(string appKey)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var appBase = await dbContext.Apps.FirstOrDefaultAsync(x => x.AppKey == appKey);
        if (appBase == null)
        {
            return;
        }
        appBase.DownloadCount += 1;
        dbContext.Apps.Update(appBase);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> ExistAsync(string appKey)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var count = await dbContext.Apps.CountAsync(x => x.AppKey == appKey);
        return count > 0;
    }

    public async Task<string> GetAppKeyFromCheckUpdateKeyAsync(string checkUpdateKey)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var appBase = await dbContext.Apps.FirstOrDefaultAsync(x => x.AppKey2 == checkUpdateKey);
        if (appBase == null)
        {
            return "";
        }
        return appBase.AppKey;
    }
}