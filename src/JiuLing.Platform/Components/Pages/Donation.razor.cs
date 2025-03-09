using Microsoft.Extensions.Options;

namespace JiuLing.Platform.Components.Pages;

public partial class Donation(IDonationService donationService, IOptions<AppSettings> options)
{
    private List<DonationDto>? _donations;

    private decimal _totalDonations => _donations?.Sum(x => x.Amount) ?? 0; // 当前总赞赏金额

    // 捐赠目标明细
    private List<DonationTarget> DonationTargets => options.Value.DonationTargets;

    // 捐赠目标明细
    private string _targetDetails => string.Join(',', DonationTargets.Select(x => $"{x.Service}：{x.Amount}").ToList());

    // 捐赠目标总金额
    private decimal _targetAmount => DonationTargets.Sum(x => x.Amount);
    // 赞赏进度
    private double _donationProgress => (double)(_totalDonations / _targetAmount * 100);

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await GetAppsAsync();
    }

    private async Task GetAppsAsync()
    {
        _donations = await donationService.GetDonationsAsync();
    }
}