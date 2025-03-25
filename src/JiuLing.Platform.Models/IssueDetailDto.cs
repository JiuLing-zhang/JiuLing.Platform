namespace JiuLing.Platform.Models;

public class IssueDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public IssueTypeEnum Type { get; set; }
    public string ReproSteps { get; set; }
    public string OS { get; set; }
    public string AppVersion { get; set; }
    public string AppName { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; }
    public string? AvatarUrl { get; set; }
    public IssueStatusEnum Status { get; set; }
    public DateTime CreateTime { get; set; }
}