using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace JiuLing.Platform.Services;
public static class CustomAuthenticationStateProviderExtends
{
    public static UserDto? UserInfo(this AuthenticationState authState)
    {
        var user = authState.User;

        if (user.Identity != null && user.Identity.IsAuthenticated)
        {
            return new UserDto
            {
                Id = int.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0,
                Email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
                Username = user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
                Role = Enum.TryParse<UserRoleEnum>(user.FindFirst(ClaimTypes.Role)?.Value, out var role) ? role : UserRoleEnum.User,
                AvatarUrl = user.FindFirst("AvatarUrl")?.Value ?? string.Empty,
                CreateTime = DateTime.TryParse(user.FindFirst("CreateTime")?.Value, out var createTime) ? createTime : DateTime.MinValue
            };
        }

        return null;
    }
}