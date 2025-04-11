namespace JiuLing.Platform.Components.Pages;

public partial class Donation(IDonationService donationService)
{
    private List<DonationUsageDto>? _donationUsages;
    private List<DonationDto>? _donations;

    // 捐赠目标总金额
    private decimal _targetAmount;
    // 当前总赞赏金额
    private decimal _totalDonations;
    // 赞赏进度
    private double _donationProgress;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await GetDonationAsync();
    }

    private async Task GetDonationAsync()
    {
        _donationUsages = await donationService.GetDonationUsagesAsync();
        _donations = await donationService.GetDonationsAsync();

        _targetAmount = _donationUsages.Sum(x => x.Amount);
        _totalDonations = _donations.Sum(x => x.Amount);
        _donationProgress = (double)(_totalDonations / _targetAmount * 100);
    }
}