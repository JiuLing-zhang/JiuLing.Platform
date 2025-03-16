namespace JiuLing.Platform.Models;

public class ForgotPasswordDto
{
    public string Token { get; set; } = null!;
    public string Email { get; set; } = null!;
}