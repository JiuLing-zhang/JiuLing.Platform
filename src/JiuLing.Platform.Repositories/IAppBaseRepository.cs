using JiuLing.Platform.Models.Entities;

namespace JiuLing.Platform.Repositories;
public interface IAppBaseRepository
{
    public Task<bool> ExistAsync(string appKey);
    public Task<List<App>> GetAllAsync();

    public Task<App?> GetOneAsync(string appKey);

    public Task<int> AddAsync(App appBase);

    public Task<int> DeleteAsync(int id);

    public Task DownloadOnceAsync(string appKey);

    public Task<string> GetAppKeyFromCheckUpdateKeyAsync(string checkUpdateKey);
}