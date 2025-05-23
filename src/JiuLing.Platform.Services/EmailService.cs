﻿using JiuLing.CommonLibs.ExtensionMethods;
using JiuLing.Platform.Common;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace JiuLing.Platform.Services;
public class EmailService(EmailSettings settings)
{
    /// <summary>
    /// 发送注册验证码邮件
    /// </summary>
    public async Task<bool> SendRegisterEmailAsync(string email, string code)
    {
        string subject = $"[{settings.DisplayName}] 注册验证码";
        string body = $"您的注册验证码为：{code}";
        return await SendEmailAsync(subject, email, body);
    }

    /// <summary>
    /// 发送忘记密码邮件
    /// </summary>
    public async Task<bool> SendForgotPasswordEmailAsync(string email, string link)
    {
        string subject = $"[{settings.DisplayName}] 用户重置密码";
        string body = $"您的重置密码链接为：{link} （5分钟内有效）";
        return await SendEmailAsync(subject, email, body);
    }

    /// <summary>
    /// 发送 Issue 创建邮件
    /// </summary>
    public async Task<bool> SendIssueCreateEmailAsync(string email, string appName, string title, string description)
    {
        string subject = $"[{settings.DisplayName}/{appName}] {title.Truncate()}";
        string body = $"{description.Truncate(200)}";
        return await SendEmailAsync(subject, email, body);
    }

    /// <summary>
    /// 发送 Issue 状态变更邮件
    /// </summary>
    public async Task<bool> SendIssueStatusChangeEmailAsync(string email, string appName, string title, IssueStatusEnum status)
    {
        string subject = $"[{settings.DisplayName}/{appName}] {title.Truncate()}";
        string body = $"当前问题的状态已更新为[{status.GetDescription()}]";
        return await SendEmailAsync(subject, email, body);
    }

    /// <summary>
    /// 发送 Issue 评论邮件
    /// </summary>
    public async Task<bool> SendIssueCommentEmailAsync(string email, string appName, string title, string comment)
    {
        string subject = $"[{settings.DisplayName}/{appName}] {title.Truncate()}";
        string body = $"{comment}";
        return await SendEmailAsync(subject, email, body);
    }

    /// <summary>
    /// 发送 Issue 标题变更邮件
    /// </summary>
    public async Task<bool> SendIssueTitleChangeEmailAsync(string email, string appName, string oldTitle, string currentTitle)
    {
        string subject = $"[{settings.DisplayName}/{appName}] {oldTitle.Truncate()}";
        string body = $"当前问题的标题已被管理员修改。原标题：{oldTitle}。新标题：{currentTitle}";
        return await SendEmailAsync(subject, email, body);
    }

    private async Task<bool> SendEmailAsync(string subject, string email, string body)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(settings.DisplayName, settings.Address));
        message.To.Add(new MailboxAddress("", email));
        message.Subject = subject;

        message.Body = new TextPart("plain")
        {
            Text = body
        };

        using var smtpClient = new SmtpClient();
        try
        {
            await smtpClient.ConnectAsync(settings.Host, settings.Port, SecureSocketOptions.StartTls);

            await smtpClient.AuthenticateAsync(settings.Username, settings.Password);

            await smtpClient.SendAsync(message);
            return true;
        }
        catch (Exception ex)
        {
            // TODO 记录日志？
            Console.WriteLine(ex);
            return false;
        }
        finally
        {
            await smtpClient.DisconnectAsync(true);
        }
    }
}