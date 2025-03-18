using JiuLing.Platform.Models.Entities;

namespace JiuLing.Platform.Repositories;

public class UserRepository(IDbContextFactory<AppDbContext> dbContextFactory) : IUserRepository
{
    public async Task<bool> CheckUserExistAsync(string email)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.Users.AnyAsync(x => x.Email == email.ToLower().Trim());
    }

    public async Task<User?> GetLoginUserAsync(string account)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.Users.FirstOrDefaultAsync(x => x.Email == account.ToLower().Trim() || x.Username == account.Trim());
    }

    public async Task<bool> CheckUsernameExistAsync(string username)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.Users.AnyAsync(x => x.Username == username.ToLower().Trim());
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email.ToLower().Trim());
    }

    public async Task AddUserAsync(User user)
    {
        user.Email = user.Email.ToLower().Trim();
        user.Username = user.Username.Trim();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        user.Email = user.Email.ToLower().Trim();
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync();
    }
    public async Task<List<User>> GetUsersAsync()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.Users.OrderBy(x => x.Id).ToListAsync();
    }
}