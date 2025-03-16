using MudBlazor;

namespace JiuLing.Platform.Components.Pages.User;

public partial class ForgotPassword(IUserService userService, ISnackbar snackbar)
{
    private string Email { get; set; } = string.Empty;

    private bool _loading;
    private bool _isFinished;
    private bool _emailError;
    private string _emailErrorText = string.Empty;

    private async Task SendResetLinkAsync()
    {
        _emailError = false;
        _emailErrorText = string.Empty;

        if (Email.IsEmpty())
        {
            _emailError = true;
            _emailErrorText = "请输入邮箱地址";
            return;
        }

        _loading = true;
        var error = await userService.RequestPasswordResetAsync(Email);
        _loading = false;

        if (error.IsNotEmpty())
        {
            _emailError = true;
            _emailErrorText = error;
            return;
        }
        _isFinished = true;
        snackbar.Add("重置链接已发送到您的邮箱，请查收。", Severity.Success);
    }
}