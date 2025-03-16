using JiuLing.Platform.Common.Enums;

namespace JiuLing.Platform.Common;
public class JwtUser
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string AvatarUrl { get; set; } = null!;
    public UserRoleEnum Role { get; set; }
    public DateTime CreateTime { get; set; }
}