namespace JiuLing.Platform.Common;

/// <summary>
/// 邮件配置
/// </summary>
public class EmailSettings
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public string Address { get; set; }
    public string DisplayName { get; set; }
}