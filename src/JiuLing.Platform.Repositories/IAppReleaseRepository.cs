using JiuLing.Platform.Models.Entities;

namespace JiuLing.Platform.Repositories;
public interface IAppReleaseRepository
{
    public Task<AppRelease?> GetLastVersionAsync(string appKey, PlatformEnum platform);
    public Task<int> AddAsync(AppRelease appRelease);

    public Task<List<AppRelease>> GetAllAsync();

    public Task<List<AppRelease>> GetAppReleaseAsync(string appKey);
}