using JiuLing.CommonLibs;
using JiuLing.Platform.Common;
using JiuLing.Platform.Common.Services;
using JiuLing.Platform.Components.Common;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;

namespace JiuLing.Platform.Components.Pages.User;

public partial class Profile(
    IUserService userService,
    ISnackbar snackbar, 
    NavigationManager navigation,
    AuthenticationStateProvider authenticationStateProvider,
    IDialogService dialog,
    JwtTokenService jwtTokenService
    )
{
    private string _currentPassword = string.Empty;
    private string _newPassword = string.Empty;
    private string _confirmPassword = string.Empty;

    private bool _savePasswordLoading;

    [CascadingParameter]
    private UserDto? UserInfo { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (UserInfo==null)
        {
            navigation.NavigateTo("/u/login", true);
        }
    }

    private async Task UploadAvatar(IBrowserFile file)
    {
        // 验证文件类型和大小
        if (!IsValidFile(file))
        {
            snackbar.Add("仅支持PNG、JPG文件，且不超过1MB", Severity.Error);
            return;
        }

        // 读取文件内容
        var buffer = new byte[file.Size];
        await using var stream = file.OpenReadStream(1024 * 1024);
        await stream.ReadAsync(buffer, 0, buffer.Length);

        var fileName = GuidUtils.GetFormatN();
        var filePath = Path.Combine("wwwroot/uploads/avatar", fileName);
        await File.WriteAllBytesAsync(filePath, buffer);

        string relativeUrl = $"/uploads/avatar/{fileName}";

        await userService.UpdateAvatarAsync(UserInfo.Email, relativeUrl);
    }

    private bool IsValidFile(IBrowserFile file)
    {
        return file.Size <= 1 * 1024 * 1024 &&
               new[] { "image/png", "image/jpeg" }.Contains(file.ContentType);
    }

    private async Task SaveUsername()
    {
        if (UserInfo.Username.IsEmpty())
        {
            snackbar.Add("用户名不能为空", Severity.Error);
            return;
        }

        if (UserInfo.Username.Length > 10)
        {
            snackbar.Add("用户名不能超过10个字符", Severity.Error);
            return;
        }

        bool? result = await dialog.ShowMessageBox(
            "提示",
            $"确认要修改用户名为【{UserInfo.Username}】吗？用户名只能修改一次",
            yesText: "确定", cancelText: "取消");
        var state = result == null ? "Canceled" : "Deleted!";

        var error = await userService.UpdateProfileAsync(UserInfo.Email, UserInfo.Username);
        if (error.IsNotEmpty())
        {
            snackbar.Add(error, Severity.Error);
            return;
        }

        var jwtUser = new JwtUser()
        {
            Id = UserInfo.Id,
            Email = UserInfo.Email,
            Username = UserInfo.Username,
            AvatarUrl = UserInfo.AvatarUrl,
            Role = UserInfo.Role,
            CreateTime = UserInfo.CreateTime
        };

        string token = jwtTokenService.GenerateToken(jwtUser);
        var customAuthProvider = (CustomAuthenticationStateProvider)authenticationStateProvider;
        var isOk = await customAuthProvider.MarkUserAsAuthenticated(token);
        if (!isOk)
        {
            snackbar.Add("授权状态更新失败", Severity.Error);
        }
        snackbar.Add("用户名修改成功", Severity.Success);
    }

    private async Task SavePassword()
    {
        if (_currentPassword.IsEmpty())
        {
            snackbar.Add("当前密码不能为空", Severity.Error);
            return;
        }
        if (_newPassword.IsEmpty())
        {
            snackbar.Add("新密码不能为空", Severity.Error);
            return;
        }
        if (_newPassword != _confirmPassword)
        {
            snackbar.Add("两次输入的密码不一致", Severity.Error);
            return;
        }
        if (_newPassword.Length < 6)
        {
            snackbar.Add("密码长度至少6位", Severity.Error);
            return;
        }
        _savePasswordLoading = true;
        var error = await userService.UpdatePasswordAsync(UserInfo.Email, _currentPassword, _newPassword);
        _savePasswordLoading = false;
        if (error.IsNotEmpty())
        {
            snackbar.Add(error, Severity.Error);
            return;
        }

        await ExitAndGotoLoginPageAsync();
    }

    private async Task ExitAndGotoLoginPageAsync()
    {
        var customAuthProvider = (CustomAuthenticationStateProvider)authenticationStateProvider;
        await customAuthProvider.MarkUserAsLoggedOut();

        navigation.NavigateTo("/u/login", true);
    }
}