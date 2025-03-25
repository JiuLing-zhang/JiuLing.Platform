namespace JiuLing.Platform.Models.Entities;

[Table("IssueComment")]
public class IssueComment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int IssueId { get; set; }
    public string Content { get; set; }
    public int UserId { get; set; }
    public DateTime CreateTime { get; set; }
}