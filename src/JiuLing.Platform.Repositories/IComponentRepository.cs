namespace JiuLing.Platform.Repositories;
public interface IComponentRepository
{
    public Task<List<Component>> GetAllAsync();
}