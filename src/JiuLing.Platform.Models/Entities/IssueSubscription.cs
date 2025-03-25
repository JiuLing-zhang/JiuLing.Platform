namespace JiuLing.Platform.Models.Entities;

[Table("IssueSubscription")]
public class IssueSubscription
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int IssueId { get; set; }
    public int UserId { get; set; }
}