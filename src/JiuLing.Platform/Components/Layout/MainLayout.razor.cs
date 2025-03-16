using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using MudBlazor;
using System.Security.Claims;

namespace JiuLing.Platform.Components.Layout;

public partial class MainLayout(
    IOptions<AppSettings> appSettingsOption,
    NavigationManager navigation,
    AuthenticationStateProvider authenticationStateProvider
    )
{
    private readonly AppSettings _appSettings = appSettingsOption.Value;
    private bool _isDarkMode;
    private MudThemeProvider _mudThemeProvider = null!;
    private MudTheme _customTheme = null!;

    private UserDto? _userInfo;
    private bool _firstRender;
    private void RedirectToLogin()
    {
        navigation.NavigateTo("/u/login");
    }
    private void RedirectToProfile()
    {
        navigation.NavigateTo("/u/profile");
    }
    private async Task Logout()
    {
        var customAuthProvider = (CustomAuthenticationStateProvider)authenticationStateProvider;
        await customAuthProvider.MarkUserAsLoggedOut();

        navigation.NavigateTo("/", true);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _customTheme = new MudTheme()
        {
            PaletteLight = new PaletteLight()
            {
                Primary = "#009688",
                Secondary = "#FF6F61",
                Background = "#F0F4F8",
                Surface = "#FFFFFF",
                TextPrimary = "#263238",
                TextSecondary = "#607D8B"
            }
        };
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _firstRender = firstRender;

            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();

            var user = authState.User;
            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                _userInfo = new UserDto
                {
                    Id = int.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : 0,
                    Email = user.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty,
                    Username = user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
                    Role = Enum.TryParse<UserRoleEnum>(user.FindFirst(ClaimTypes.Role)?.Value, out var role) ? role : UserRoleEnum.User,
                    AvatarUrl = user.FindFirst("AvatarUrl")?.Value ?? string.Empty,
                    CreateTime = DateTime.TryParse(user.FindFirst("CreateTime")?.Value, out var createTime) ? createTime : DateTime.MinValue
                };
            }
            else
            {
                _userInfo = null;
            }
            await InvokeAsync(StateHasChanged);
        }
    }
}