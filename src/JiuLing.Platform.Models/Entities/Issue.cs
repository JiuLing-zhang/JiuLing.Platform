namespace JiuLing.Platform.Models.Entities;

[Table("Issue")]
public class Issue
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public IssueTypeEnum Type { get; set; }   // 功能建议、错误报告
    public string ReproSteps { get; set; } = null!; // 仅用于错误报告
    public string OS { get; set; } = null!;// 仅用于错误报告
    public string AppVersion { get; set; } = null!; // 仅用于错误报告
    public IssueStatusEnum Status { get; set; }
    public string AppKey { get; set; } = null!;
    public int UserId { get; set; }
    public DateTime CreateTime { get; set; }
}