namespace JiuLing.Platform.Models;

/// <summary>
/// 打赏用途
/// </summary>
public class DonationUsageDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Amount { get; set; }
}