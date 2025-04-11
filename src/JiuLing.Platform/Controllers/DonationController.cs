using Microsoft.AspNetCore.Mvc;

namespace JiuLing.Platform.Controllers;

[Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
[ApiController]
public class DonationController(IDonationService donationService) : ControllerBase
{
    [HttpGet("list")]
    public async Task<IActionResult> GetDonationsAsync()
    {
        var donations = await donationService.GetDonationsAsync();
        return Ok(donations);
    }
}