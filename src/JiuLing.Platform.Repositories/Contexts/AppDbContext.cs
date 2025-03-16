namespace JiuLing.Platform.Repositories.Contexts;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<App> Apps { get; set; } = null!;
    public DbSet<AppRelease> AppReleases { get; set; } = null!;
    public DbSet<ConfigBase> ConfigBase { get; set; } = null!;
    public DbSet<Donation> Donations { get; set; } = null!;

    public DbSet<User> Users { get; set; } = null!;
}