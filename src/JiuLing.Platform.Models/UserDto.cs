using JiuLing.Platform.Common.Enums;

namespace JiuLing.Platform.Models;
public record UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string AvatarUrl { get; set; } = null!;
    public UserRoleEnum Role { get; set; }
    public DateTime CreateTime { get; set; }
    public string Token { get; set; } = null!;
    public bool IsEnabled { get; set; }
}