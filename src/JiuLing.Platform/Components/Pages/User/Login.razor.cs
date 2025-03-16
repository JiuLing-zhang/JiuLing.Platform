using Microsoft.AspNetCore.Components.Authorization;

namespace JiuLing.Platform.Components.Pages.User;

public partial class Login(
    IUserService userService,
    NavigationManager navigation,
    AuthenticationStateProvider authenticationStateProvider)
{
    private string Account { get; set; } = string.Empty;
    private string Password { get; set; } = string.Empty;

    private bool _accountError;
    private string _accountErrorText = string.Empty;
    private bool _passwordError;
    private string _passwordErrorText = string.Empty;

    private bool _loading;
    private async Task LoginAsync()
    {
        _loading = true;
        await InvokeAsync(StateHasChanged);

        try
        {
            // 重置错误状态
            ResetErrors();

            if (Account.IsEmpty())
            {
                _accountError = true;
                _accountErrorText = "请输入邮箱地址";
            }
            if (Password.IsEmpty())
            {
                _passwordError = true;
                _passwordErrorText = "请输入密码";
            }

            if (_accountError || _passwordError)
            {
                return;
            }
            var (error, user) = await userService.LoginAsync(Account, Password);

            if (error.IsNotEmpty())
            {
                _passwordError = true;
                _passwordErrorText = error;
                return;
            }

            // 登录成功，更新认证状态并跳转
            var customAuthProvider = (CustomAuthenticationStateProvider)authenticationStateProvider;
            var isOk = await customAuthProvider.MarkUserAsAuthenticated(user!.Token);
            if (!isOk)
            {
                _passwordError = true;
                _passwordErrorText = "授权状态更新失败";
                return;
            }
            navigation.NavigateTo("/", true);
        }
        finally
        {
            _loading = false;
        }
    }

    private void ResetErrors()
    {
        _accountError = false;
        _accountErrorText = string.Empty;
        _passwordError = false;
        _passwordErrorText = string.Empty;
    }
}