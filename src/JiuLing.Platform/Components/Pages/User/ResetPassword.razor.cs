using MudBlazor;

namespace JiuLing.Platform.Components.Pages.User;

public partial class ResetPassword(NavigationManager navigation, IUserService userService, ISnackbar snackbar)
{
    private string NewPassword { get; set; } = string.Empty;
    private string ConfirmPassword { get; set; } = string.Empty;

    private bool _loading;
    private bool _passwordError;
    private string _passwordErrorText = string.Empty;
    private bool _confirmPasswordError;
    private string _confirmPasswordErrorText = string.Empty;

    [Parameter]
    [SupplyParameterFromQuery(Name = "token")]
    public string? Token { get; set; }

    private async Task ResetPasswordAsync()
    {
        ResetErrors();

        if (NewPassword.IsEmpty())
        {
            _passwordError = true;
            _passwordErrorText = "请输入新密码";
            return;
        }
        if (ConfirmPassword.IsEmpty())
        {
            _confirmPasswordError = true;
            _confirmPasswordErrorText = "请确认新密码";
            return;
        }

        if (NewPassword != ConfirmPassword)
        {
            _confirmPasswordError = true;
            _confirmPasswordErrorText = "两次输入的密码不一致";
            return;
        }

        if (ConfirmPassword.Length < 6)
        {
            _confirmPasswordError = true;
            _confirmPasswordErrorText = "密码长度至少6位";
            return;
        }

        _loading = true;
        await InvokeAsync(StateHasChanged);
        try
        {
            var error = await userService.ResetPasswordAsync(Token!, NewPassword);
            if (error.IsNotEmpty())
            {
                _confirmPasswordError = true;
                _confirmPasswordErrorText = error;
                return;
            }

            snackbar.Add("设置成功，请使用新密码登录。", Severity.Success);
            navigation.NavigateTo("/u/login");
        }
        finally
        {
            _loading = false;
        }
    }

    private void ResetErrors()
    {
        _passwordError = false;
        _passwordErrorText = string.Empty;
        _confirmPasswordError = false;
        _confirmPasswordErrorText = string.Empty;
    }
}