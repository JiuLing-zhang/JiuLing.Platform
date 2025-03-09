namespace JiuLing.Platform.Models;

public class DonationDto
{
    public string User { get; set; }
    public DateTime Time { get; set; }
    public decimal Amount { get; set; }
    public string? Message { get; set; }
    public string? Note { get; set; }
}