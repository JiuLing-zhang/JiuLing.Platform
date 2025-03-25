namespace JiuLing.Platform.Models;

public class IssueSubscriberDto
{
    public int IssueId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; }
    public string? AvatarUrl { get; set; }
    public string Email { get; set; }
}