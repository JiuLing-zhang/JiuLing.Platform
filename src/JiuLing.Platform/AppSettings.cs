namespace JiuLing.Platform;
public class AppSettings
{
    public string VirusTotalApiKey { get; set; } = null!;
    public string StunServer { get; set; } = null!;

    /// <summary>
    /// 备案号
    /// </summary>
    public string BeiAn { get; set; } = null!;
    public AppSettingEmail Email { get; set; } = null!;
}

public class AppSettingEmail
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public string Address { get; set; }
    public string DisplayName { get; set; }
}