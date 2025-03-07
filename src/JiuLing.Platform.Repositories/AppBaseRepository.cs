namespace JiuLing.Platform.Repositories;
public class AppBaseRepository(IDbContextFactory<AppDbContext> dbContextFactory) : IAppBaseRepository
{
    public async Task<List<AppBase>> GetAllAsync()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.AppBase.OrderBy(x => x.Sort).ToListAsync();
    }

    public async Task<AppBase?> GetOneAsync(string appKey)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.AppBase.FirstOrDefaultAsync(x => x.AppKey == appKey && x.IsShow);
    }

    public async Task<int> AddAsync(AppBase appBase)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.AppBase.Add(appBase);
        return await dbContext.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(int id)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var appBase = await dbContext.AppBase.FindAsync(id);
        if (appBase == null)
        {
            return 0;
        }
        dbContext.AppBase.Remove(appBase);
        return await dbContext.SaveChangesAsync();
    }

    public async Task DownloadOnceAsync(string appKey)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var appBase = await dbContext.AppBase.FirstOrDefaultAsync(x => x.AppKey == appKey);
        if (appBase == null)
        {
            return;
        }
        appBase.DownloadCount += 1;
        dbContext.AppBase.Update(appBase);
        await dbContext.SaveChangesAsync();
    }

    public async Task<bool> ExistAsync(string appKey)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var count = await dbContext.AppBase.CountAsync(x => x.AppKey == appKey);
        return count > 0;
    }

    public async Task<string> GetAppKeyFromCheckUpdateKeyAsync(string checkUpdateKey)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var appBase = await dbContext.AppBase.FirstOrDefaultAsync(x => x.AppKey2 == checkUpdateKey);
        if (appBase == null)
        {
            return "";
        }
        return appBase.AppKey;
    }
}