using JiuLing.CommonLibs;
using JiuLing.Platform.Common;
using JiuLing.Platform.Common.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Options;
using MudBlazor;

namespace JiuLing.Platform.Components.Pages.User;

public partial class Profile(
    IUserService userService,
    ISnackbar snackbar,
    NavigationManager navigation,
    AuthenticationStateProvider authenticationStateProvider,
    JwtTokenService jwtTokenService,
    IWebHostEnvironment env,
    IOptions<FilePathConfig> paths
    )
{

    private readonly FilePathConfig _paths = paths.Value;
    private bool _loading;
    private const int ImageMaxLength = 1 * 1024 * 1024;
    private string _currentPassword = string.Empty;
    private string _newPassword = string.Empty;
    private string _confirmPassword = string.Empty;

    private bool _savePasswordLoading;

    private UserDto? _user;
    protected override async Task OnInitializedAsync()
    {
        if (RendererInfo.IsInteractive)
        {
            var authState = await ((CustomAuthenticationStateProvider)authenticationStateProvider).GetAuthenticationStateAsync();
            _user = authState.UserInfo();

            if (_user == null)
            {
                navigation.NavigateTo("/u/login", true);
                return;
            }
        }
        await base.OnInitializedAsync();
    }

    private async Task UploadAvatar(IBrowserFile? file)
    {
        if (file == null)
        {
            snackbar.Add("上传失败：未知错误", Severity.Error);
            return;
        }

        var buffer = await CheckAndGetImageBuffer(file);
        if (buffer == null)
        {
            snackbar.Add("仅支持PNG、JPG文件，且不超过1MB", Severity.Error);
            return;
        }
        _loading = true;
        await InvokeAsync(StateHasChanged);

        var physicalPath = Path.Combine(env.WebRootPath, _paths.Avatars);
        if (!Directory.Exists(physicalPath))
        {
            Directory.CreateDirectory(physicalPath);
        }
        var fileExtension = Path.GetExtension(file.Name).ToLower();
        var fileName = GuidUtils.GetFormatN() + fileExtension;

        var filePath = Path.Combine(physicalPath, fileName);
        await File.WriteAllBytesAsync(filePath, buffer);

        string relativeUrl = $"/{_paths.Avatars}/{fileName}";

        _user!.AvatarUrl = relativeUrl;
        var error = await userService.UpdateUserAvatarAsync(_user.Email, relativeUrl);
        if (error.IsNotEmpty())
        {
            snackbar.Add(error, Severity.Error);
            return;
        }

        var jwtUser = new JwtUser()
        {
            Id = _user.Id,
            Email = _user.Email,
            Username = _user.Username,
            AvatarUrl = relativeUrl,
            Role = _user.Role,
            CreateTime = _user.CreateTime
        };

        string token = jwtTokenService.GenerateToken(jwtUser);
        var customAuthProvider = (CustomAuthenticationStateProvider)authenticationStateProvider;
        var isOk = await customAuthProvider.MarkUserAsAuthenticated(token);
        if (!isOk)
        {
            snackbar.Add("授权状态更新失败", Severity.Error);
        }
        snackbar.Add("用户名修改成功", Severity.Success);

        _loading = false;
        await InvokeAsync(StateHasChanged);
        navigation.NavigateTo(navigation.Uri, true);
    }

    private async Task<byte[]?> CheckAndGetImageBuffer(IBrowserFile file)
    {
        try
        {
            if (file.Size > ImageMaxLength)
            {
                return null;
            }

            if (file.ContentType != "image/jpeg" && file.ContentType != "image/png")
            {
                return null;
            }

            // 读取文件头
            using var memoryStream = new MemoryStream();
            await file.OpenReadStream(ImageMaxLength).CopyToAsync(memoryStream);
            var buffer = memoryStream.ToArray();

            // 检查文件头是否为JPG或PNG
            if (buffer[0] == 0xFF && buffer[1] == 0xD8) // JPEG文件头
            {
                return buffer;
            }
            if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47) // PNG文件头
            {
                return buffer;
            }
            return null;
        }
        catch
        {
            return null;
        }
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
        var error = await userService.UpdatePasswordAsync(_user!.Email, _currentPassword, _newPassword);
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