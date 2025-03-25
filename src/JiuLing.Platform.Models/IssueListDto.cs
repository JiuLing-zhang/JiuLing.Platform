namespace JiuLing.Platform.Models;
public class IssueListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public IssueTypeEnum Type { get; set; }   // 功能建议、错误报告   
    public IssueStatusEnum Status { get; set; }
    public string AppName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public DateTime CreateTime { get; set; }
}