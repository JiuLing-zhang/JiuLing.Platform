namespace JiuLing.Platform.Repositories;
public class ConfigBaseRepository(IDbContextFactory<AppDbContext> dbContextFactory) : IConfigBaseRepository
{
    public async Task<T?> GetOneAsync<T>(string key)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var config = await dbContext.ConfigBase.FindAsync(key);
        if (config == null)
        {
            return default;
        }
        return System.Text.Json.JsonSerializer.Deserialize<T>(config.ConfigDetail);
    }

    public async Task<string> GetOneAsync(string key)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var config = await dbContext.ConfigBase.FindAsync(key);
        if (config == null)
        {
            return "";
        }
        return config.ConfigDetail;
    }
}
