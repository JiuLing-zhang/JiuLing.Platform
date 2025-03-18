namespace JiuLing.Platform.Models.Entities;

[Table("User")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string AvatarUrl { get; set; } = null!;
    public bool IsEnabled { get; set; }
    public UserRoleEnum Role { get; set; }
    public DateTime CreateTime { get; set; }
}