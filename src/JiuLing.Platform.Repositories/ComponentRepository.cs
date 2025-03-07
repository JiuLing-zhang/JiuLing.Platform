namespace JiuLing.Platform.Repositories;
public class ComponentRepository(IDbContextFactory<AppDbContext> dbContextFactory) : IComponentRepository
{
    public async Task<List<Component>> GetAllAsync()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.Component.OrderBy(x => x.Id).ToListAsync();
    }
}