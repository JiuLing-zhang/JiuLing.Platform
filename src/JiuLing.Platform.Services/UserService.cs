using JiuLing.CommonLibs;
using JiuLing.CommonLibs.Security;
using JiuLing.Platform.Common;
using JiuLing.Platform.Common.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;

namespace JiuLing.Platform.Services;
public class UserService(
    IUserRepository userRepository,
    EmailService emailService,
    IMemoryCache memoryCache,
    JwtTokenService jwtTokenService,
    NavigationManager navigationManager
    ) : IUserService
{
    public async Task<bool> CheckUsernameExistAsync(string username)
    {
        return await userRepository.CheckUsernameExistAsync(username);
    }

    public async Task<bool> CheckUserExistAsync(string email)
    {
        return await userRepository.CheckUserExistAsync(email);
    }

    public async Task<string> SendRegisterCodeAsync(string email)
    {
        // 缓存中如果已经有验证码，则直接使用
        if (!memoryCache.TryGetValue($"{CachePrefix.Register}{email}", out string? code))
        {
            code = RandomUtils.GetOneByLength(4);
        }

        if (!await emailService.SendRegisterEmailAsync(email, code!))
        {
            return "邮件发送失败";
        }
        TimeSpan expiry = TimeSpan.FromMinutes(5);
        memoryCache.Set($"{CachePrefix.Register}{email}", code, expiry);
        return "";
    }

    public Task<string> CheckRegisterCodeAsync(string email, string verificationCode)
    {
        if (!memoryCache.TryGetValue($"{CachePrefix.Register}{email}", out string? correctCode))
        {
            return Task.FromResult("验证码无效或已过期");
        }

        if (correctCode != verificationCode)
        {
            return Task.FromResult("验证码无效或已过期");
        }
        return Task.FromResult("");
    }

    public async Task<string> RegisterAsync(string username, string email, string password, string verificationCode)
    {
        if (!memoryCache.TryGetValue($"{CachePrefix.Register}{email}", out string? correctCode))
        {
            return "验证码无效或已过期";
        }

        if (correctCode != verificationCode)
        {
            return "验证码无效或已过期";

        }

        if (await userRepository.CheckUsernameExistAsync(username))
        {
            return "用户名已存在";
        }

        if (await userRepository.CheckUserExistAsync(email))
        {
            return "已完成注册，请直接登录";
        }

        var passwordHash = HashPassword(password);
        var user = new User()
        {
            Email = email,
            Username = username,
            Password = passwordHash,
            AvatarUrl = "",
            Role = UserRoleEnum.User,
            IsEnabled = true,
            CreateTime = DateTime.Now
        };
        await userRepository.AddUserAsync(user);

        memoryCache.Remove($"{CachePrefix.Register}{email}");
        return "";
    }

    public async Task<(string Error, UserDto? User)> LoginAsync(string account, string password)
    {
        // 这里暂时把错误提示分开，后续如果有安全威胁再做统一处理
        var user = await userRepository.GetLoginUserAsync(account);
        if (user == null)
        {
            return ("用户不存在", null);
        }

        if (!user.IsEnabled)
        {
            return ("用户已被禁用", null);
        }

        var passwordHash = HashPassword(password);
        if (user.Password != passwordHash)
        {
            return ("密码错误", null);
        }

        var jwtUser = new JwtUser()
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            AvatarUrl = user.AvatarUrl,
            Role = user.Role,
            CreateTime = user.CreateTime
        };

        string token = jwtTokenService.GenerateToken(jwtUser);

        var userDto = new UserDto()
        {
            Id = user.Id,
            Role = user.Role,
            Email = user.Email,
            Username = user.Username,
            AvatarUrl = user.AvatarUrl,
            CreateTime = user.CreateTime,
            Token = token,
        };

        return ("", userDto);
    }

    public async Task<string> RequestPasswordResetAsync(string email)
    {
        var user = await userRepository.GetUserByEmailAsync(email);
        if (user == null)
        {
            return "用户不存在";
        }

        if (!user.IsEnabled)
        {
            return "用户已被禁用";
        }

        var cacheDto = new ForgotPasswordDto()
        {
            Email = email,
            Token = GuidUtils.GetFormatN()
        };


        var resetLink = $"{navigationManager.BaseUri}u/reset-password?token={cacheDto.Token}";
        if (!await emailService.SendForgotPasswordEmailAsync(email, resetLink))
        {
            return "邮件发送失败";
        }

        TimeSpan expiry = TimeSpan.FromMinutes(5);
        memoryCache.Set($"{CachePrefix.ForgotPassword}{cacheDto.Token}", cacheDto, expiry);

        return "";
    }

    public async Task<string> ResetPasswordAsync(string token, string password)
    {
        if (!memoryCache.TryGetValue($"{CachePrefix.ForgotPassword}{token}", out ForgotPasswordDto? cache))
        {
            return "重置链接无效或已过期";
        }

        var user = await userRepository.GetUserByEmailAsync(cache!.Email);
        if (user == null)
        {
            return "用户不存在";
        }
        var passwordHash = HashPassword(password);
        user.Password = passwordHash;

        await userRepository.UpdateUserAsync(user);

        memoryCache.Remove($"{CachePrefix.ForgotPassword}{token}");
        return "";
    }

    public async Task<string> UpdateUserAsync(UserDto user)
    {
        var user2 = await userRepository.GetUserByEmailAsync(user.Email);
        if (user2 == null)
        {
            return "用户不存在";
        }

        user2.IsEnabled = user.IsEnabled;
        user2.AvatarUrl = user.AvatarUrl;
        await userRepository.UpdateUserAsync(user2);
        return "";
    }


    public async Task<string> UpdatePasswordAsync(string email, string currentPassword, string newPassword)
    {
        var user = await userRepository.GetUserByEmailAsync(email);
        if (user == null)
        {
            return "用户不存在";
        }
        var currentPasswordHash = HashPassword(currentPassword);
        if (currentPasswordHash != user.Password)
        {
            return "密码错误";
        }

        var newPasswordHash = HashPassword(newPassword);
        user.Password = newPasswordHash;
        await userRepository.UpdateUserAsync(user);
        return "";
    }

    public async Task<List<UserDto>> GetUsersAsync()
    {
        var users = await userRepository.GetUsersAsync();
        return users.Select(x => new UserDto()
        {
            Username = x.Username,
            Email = x.Email,
            AvatarUrl = x.AvatarUrl,
            CreateTime = x.CreateTime,
            Role = x.Role,
            IsEnabled = x.IsEnabled,
        }).ToList();
    }

    private string HashPassword(string password)
    {
        password = MD5Utils.GetStringValueToLower(password);
        password = MD5Utils.GetStringValueToLower(password);
        return password;
    }
}