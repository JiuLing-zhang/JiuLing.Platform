﻿namespace JiuLing.Platform.Services;
public interface IUserService
{
    Task<bool> CheckUsernameExistAsync(string username);
    Task<bool> CheckUserExistAsync(string email);
    Task<string> SendRegisterCodeAsync(string email);
    Task<string> CheckRegisterCodeAsync(string email, string verificationCode);
    Task<string> RegisterAsync(string username, string email, string password, string verificationCode);
    Task<(string Error, UserDto? User)> LoginAsync(string account, string password);
    Task<string> RequestPasswordResetAsync(string email);
    Task<string> ResetPasswordAsync(string token, string password);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<string> UpdateUserAvatarAsync(string email, string avatarUrl);
    Task<string> ToggleUserStatusAsync(string email, bool enabled);
    Task<string> UpdatePasswordAsync(string email, string currentPassword, string newPassword);
    Task<List<UserDto>> GetUsersAsync();
}