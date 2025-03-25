using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;

namespace JiuLing.Platform.Components.Pages.Admin;

public partial class Users(
    AuthenticationStateProvider authenticationStateProvider,
    NavigationManager navigation,
    IUserService userService,
    ISnackbar snackbar)
{
    private bool _loading;
    private List<UserDto> _users = [];

    protected override async Task OnInitializedAsync()
    {
        if (RendererInfo.IsInteractive)
        {
            var authState = await ((CustomAuthenticationStateProvider)authenticationStateProvider).GetAuthenticationStateAsync();
            var user = authState.UserInfo();

            if (user == null || user.Role != UserRoleEnum.Admin)
            {
                navigation.NavigateTo("/u/login", true);
            }
            _loading = true;
            await InvokeAsync(StateHasChanged);
            _users = await userService.GetUsersAsync();
            _loading = false;
        }
    }

    private async Task ToggleUserStatus(UserDto user)
    {
        _loading = true;
        // 切换用户状态
        user.IsEnabled = !user.IsEnabled;
        await userService.ToggleUserStatusAsync(user.Email, user.IsEnabled);

        _users = await userService.GetUsersAsync();
        _loading = false;
        snackbar.Add($"用户 {user.Username} 状态已更新", Severity.Success);
    }
}