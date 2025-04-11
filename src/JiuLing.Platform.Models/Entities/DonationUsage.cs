namespace JiuLing.Platform.Models.Entities;

/// <summary>
/// 打赏用途
/// </summary>
[Table("DonationUsage")]
public class DonationUsage
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
}