using Microsoft.EntityFrameworkCore;

namespace JiuLing.Platform.Repositories.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<AppBase> AppBase { get; set; } = null!;
    public DbSet<AppRelease> AppRelease { get; set; } = null!;
    public DbSet<Component> Component { get; set; } = null!;
    public DbSet<ConfigBase> ConfigBase { get; set; } = null!;
}