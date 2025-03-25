namespace JiuLing.Platform.Models;

public class IssueDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public IssueTypeEnum Type { get; set; }   // 功能建议、错误报告
    public string? ReproSteps { get; set; } // 仅用于错误报告
    public string? OS { get; set; }  // 仅用于错误报告
    public string? AppVersion { get; set; }   // 仅用于错误报告
    public IssueStatusEnum Status { get; set; }
    public string AppKey { get; set; } = null!;
    public int UserId { get; set; }
    public bool SubscribeToUpdates { get; set; }
    public DateTime CreateTime { get; set; }
}