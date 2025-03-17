using Microsoft.Extensions.Options;
using MudExtensions;

namespace JiuLing.Platform.Components.Pages.User;

public partial class Register(
    IUserService userService,
    NavigationManager navigation,
    IOptions<AppSettings> appSettingsOption)
{

    private MudStepperExtended _stepper = new();
    // 当前步骤索引
    private int _activeStep;
    private string _serverEmail = "";

    private string Username { get; set; } = string.Empty;
    private string Email { get; set; } = string.Empty;
    private string Password { get; set; } = string.Empty;
    private string VerificationCode { get; set; } = string.Empty;

    // 邮箱验证码倒计时
    private int _countdown;

    // 错误状态和错误消息
    private bool _usernameError;
    private string _usernameErrorText = string.Empty;
    private bool _emailError;
    private string _emailErrorText = string.Empty;
    private bool _passwordError;
    private string _passwordErrorText = string.Empty;
    private bool _verificationCodeError;
    private string _verificationCodeErrorText = string.Empty;
    private bool _loading;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            // 隐藏邮箱展示，低端反爬
            _serverEmail = appSettingsOption.Value.Email.Username;
            await InvokeAsync(StateHasChanged);
        }
    }

    private async Task GoToNextStep()
    {
        _loading = true;
        await InvokeAsync(StateHasChanged);
        try
        {
            switch (_activeStep)
            {
                case 0: // 创建用户

                    _usernameError = false;
                    _usernameErrorText = "";

                    if (!await CheckUsernameAsync())
                    {
                        return;
                    }
                    break;

                case 1: // 验证邮箱

                    _emailError = false;
                    _emailErrorText = "";
                    _verificationCodeError = false;
                    _verificationCodeErrorText = "";

                    if (!await CheckEmailAsync())
                    {
                        return;
                    }

                    if (!await CheckVerificationCodeAsync())
                    {
                        return;
                    }
                    break;

                case 2: // 设置密码
                    if (!await RegisterAsync())
                    {
                        return;
                    }
                    navigation.NavigateTo("/u/login");
                    break;
            }

            // 进入下一步
            _activeStep++;
            await _stepper.SetActiveStepByIndex(_activeStep);
        }
        finally
        {
            _loading = false;
        }
    }
    private async Task<bool> CheckUsernameAsync()
    {
        if (Username.IsEmpty())
        {
            _usernameError = true;
            _usernameErrorText = "请输入用户名";
            return false;
        }

        if (Username.Length < 2 || Username.Length > 20)
        {
            _usernameError = true;
            _usernameErrorText = "用户名不能少于2位、超过20位";
            return false;
        }

        if (await userService.CheckUsernameExistAsync(Username))
        {
            _emailError = true;
            _emailErrorText = "用户名已使用";
            return false;
        }
        return true;
    }

    private async Task<bool> CheckEmailAsync()
    {
        if (Email.IsEmpty())
        {
            _emailError = true;
            _emailErrorText = "请输入邮箱地址";
            return false;
        }

        if (await userService.CheckUserExistAsync(Email))
        {
            _emailError = true;
            _emailErrorText = "邮箱已被注册";
            return false;
        }
        return true;
    }

    private async Task<bool> CheckVerificationCodeAsync()
    {
        if (VerificationCode.IsEmpty())
        {
            _verificationCodeError = true;
            _verificationCodeErrorText = "请输入验证码";
            return false;
        }

        var error = await userService.CheckRegisterCodeAsync(Email, VerificationCode);
        if (error.IsNotEmpty())
        {
            _verificationCodeError = true;
            _verificationCodeErrorText = error;
            return false;
        }
        return true;
    }

    private async Task SendVerificationCodeAsync()
    {
        // 重置错误状态和错误消息
        _verificationCodeError = false;
        _verificationCodeErrorText = "";

        // 开始倒计时
        _countdown = 60;
        await InvokeAsync(StateHasChanged);

        // 调用服务层发送验证码
        var error = await userService.SendRegisterCodeAsync(Email);
        if (error.IsNotEmpty())
        {
            _verificationCodeError = true;
            _verificationCodeErrorText = error;
            _countdown = 0;
            return;
        }

        while (_countdown > 0)
        {
            await Task.Delay(1000);
            _countdown--;
            StateHasChanged();
        }
    }

    private async Task<bool> RegisterAsync()
    {
        if (Password.IsEmpty())
        {
            _passwordError = true;
            _passwordErrorText = "请输入密码";
            return false;
        }

        if (Password.Length < 6)
        {
            _passwordError = true;
            _passwordErrorText = "密码长度至少6位";
            return false;
        }

        var error = await userService.RegisterAsync(Username, Email, Password, VerificationCode);
        if (error.IsNotEmpty())
        {
            _passwordError = true;
            _passwordErrorText = error;
            return false;
        }
        return true;
    }
}