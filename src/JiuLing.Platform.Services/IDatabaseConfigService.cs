namespace JiuLing.Platform.Services;
public interface IDatabaseConfigService
{
    public Task<T?> GetOneAsync<T>(string key);
    public Task<string> GetOneAsync(string key);
}