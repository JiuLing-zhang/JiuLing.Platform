using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;
using MudBlazor;

namespace JiuLing.Platform.Components.Layout;

public partial class MainLayout(
    IOptions<AppSettings> appSettingsOption,
    NavigationManager navigation,
    AuthenticationStateProvider authenticationStateProvider,
    ISnackbar snackbar
    )
{
    private UserDto? _user;
    private readonly AppSettings _appSettings = appSettingsOption.Value;
    private bool _isDarkMode;
    private MudThemeProvider _mudThemeProvider = null!;
    private MudTheme _customTheme = null!;
    JiuLing.CommonLibs.Log.ILogger _logger = JiuLing.CommonLibs.Log.LogManager.GetLogger();

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
                Secondary = "#00796B",
                Tertiary = "#8BC34A",
                Background = "#F0F4F8",
                Surface = "#FFFFFF",
                TextPrimary = "#263238",
                TextSecondary = "#607D8B"
            }
        };

        if (RendererInfo.IsInteractive)
        {
            var authState = await ((CustomAuthenticationStateProvider)authenticationStateProvider).GetAuthenticationStateAsync();
            _user = authState.UserInfo();
        }
    }

    private void HandleError(Exception ex)
    {
        snackbar.Add($"出错咯：{ex.Message}", Severity.Error);
        _logger.Write($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
    }
}