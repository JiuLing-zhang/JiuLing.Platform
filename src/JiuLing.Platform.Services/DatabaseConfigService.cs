namespace JiuLing.Platform.Services;
public class DatabaseConfigService(IConfigBaseRepository configBaseRepository) : IDatabaseConfigService
{
    public async Task<T?> GetOneAsync<T>(string key)
    {
        return await configBaseRepository.GetOneAsync<T>(key);
    }

    public async Task<string> GetOneAsync(string key)
    {
        return await configBaseRepository.GetOneAsync(key);
    }
}