namespace JiuLing.Platform.Models;
public class IssueCommentDto
{
    public int Id { get; set; }
    public int IssueId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; }
    public string? AvatarUrl { get; set; }
    public string Content { get; set; }
    public DateTime CreateTime { get; set; }
}