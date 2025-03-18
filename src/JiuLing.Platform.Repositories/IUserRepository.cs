using JiuLing.Platform.Models.Entities;

namespace JiuLing.Platform.Repositories;

public interface IUserRepository
{
    Task<bool> CheckUsernameExistAsync(string username);
    Task<bool> CheckUserExistAsync(string email);
    Task<User?> GetLoginUserAsync(string account);
    Task<User?> GetUserByEmailAsync(string email);
    Task AddUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task<List<User>> GetUsersAsync();
}