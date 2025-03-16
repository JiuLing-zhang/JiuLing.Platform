using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace JiuLing.Platform.Services;
public class EmailService(string host, int port, string username, string password, string displayName)
{
    public async Task<bool> SendRegisterEmailAsync(string email, string code)
    {
        string subject = $"[{displayName}] 注册验证码";
        string body = $"您的注册验证码为：{code}";
        return await SendEmailAsync(subject, email, body);
    }

    public async Task<bool> SendForgotPasswordEmailAsync(string email, string link)
    {
        string subject = $"[{displayName}] 用户重置密码";
        string body = $"您的重置密码链接为：{link} （5分钟内有效）";
        return await SendEmailAsync(subject, email, body);
    }

    private async Task<bool> SendEmailAsync(string subject, string email, string body)
    {
#if DEBUG
        return true;
#endif
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(displayName, username));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = subject;

        message.Body = new TextPart("plain")
        {
            Text = body
        };

        using var smtpClient = new SmtpClient();
        try
        {
            await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);

            await smtpClient.AuthenticateAsync(username, password);

            await smtpClient.SendAsync(message);
            return true;
        }
        catch (Exception ex)
        {
            // TODO 记录日志？
            return false;
        }
        finally
        {
            await smtpClient.DisconnectAsync(true);
        }
    }
}