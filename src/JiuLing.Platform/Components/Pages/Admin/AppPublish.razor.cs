using System.Security.Cryptography;
using JiuLing.CommonLibs;
using JiuLing.Platform.Common;
using JiuLing.Platform.Components.Common;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace JiuLing.Platform.Components.Pages.Admin;

public partial class AppPublish(
    AuthenticationStateProvider authenticationStateProvider,
    NavigationManager navigation,
    IWebHostEnvironment env,
    IAppService appService,
    IDialogService dialog,
    FilePathConfig filePaths
    )
{
    private List<AppDetailDto>? _apps;
    private readonly AppPublishDto _model = new();
    private IBrowserFile? _file;

    private MudForm _form = null!;

    private bool _isPublishing;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        if (RendererInfo.IsInteractive)
        {
            var authState = await ((CustomAuthenticationStateProvider)authenticationStateProvider).GetAuthenticationStateAsync();
            var user = authState.UserInfo();

            if (user == null || user.Role != UserRoleEnum.Admin)
            {
                navigation.NavigateTo("/u/login", true);
            }

            await GetAppsAsync();
        }
    }

    private async Task GetAppsAsync()
    {
        _apps = await appService.GetAppNamesAsync();
    }
    private async Task PublishAsync()
    {
        await _form.Validate();
        if (!_form.IsValid)
        {
            return;
        }

        try
        {
            _isPublishing = true;

            if (!Version.TryParse(_model.VersionName, out _))
            {
                await dialog.ShowInfoAsync("版本号格式不正确");
                return;
            }

            if (!await appService.AllowPublishAsync(_model.AppKey, _model.Platform, _model.VersionName))
            {
                await dialog.ShowInfoAsync("版本号不能小于历史版本");
                return;
            }

            var random = RandomUtils.GetOneByLength(4);
            var fileName = $"{_model.AppKey}_{_model.Platform}_{_model.VersionName}_{random}".ToLower();

            var fileExtension = Path.GetExtension(_file!.Name);
            fileName = $"{fileName}{fileExtension}";

            var relativeUrl = $"/{filePaths.Apps}/{fileName}";
            var physicalPath = Path.Combine(env.WebRootPath, filePaths.Apps);
            if (!Directory.Exists(physicalPath))
            {
                Directory.CreateDirectory(physicalPath);
            }
            var path = Path.Combine(physicalPath, fileName);

            HashAlgorithm hashAlgorithm;
            switch (_model.SignType)
            {
                case SignTypeEnum.MD5:
                    hashAlgorithm = MD5.Create();
                    break;
                case SignTypeEnum.SHA1:
                    hashAlgorithm = SHA1.Create();
                    break;
                case SignTypeEnum.None:
                default:
                    await dialog.ShowInfoAsync("错误的签名方式");
                    return;
            }

            string signValue;
            await using FileStream fs = new(path, FileMode.Create);
            using (hashAlgorithm)
            {
                var fileStream = _file.OpenReadStream(long.MaxValue);
                await fileStream.CopyToAsync(fs);

                var fileHash = await hashAlgorithm.ComputeHashAsync(fileStream);
                signValue = Convert.ToHexStringLower(fileHash);
            }

            var model = new AppReleaseDto()
            {
                AppKey = _model.AppKey,
                Platform = _model.Platform,
                VersionName = _model.VersionName,
                IsMinVersion = _model.IsMinVersion,
                UpgradeLog = _model.Log,
                FilePath = relativeUrl,
                FileLength = (int)_file.Size,
                SignType = _model.SignType,
                SignValue = signValue
            };
            var result = await appService.PublishAsync(model);
            if (result)
            {
                await dialog.ShowInfoAsync("发布成功");
                return;
            }
            await dialog.ShowInfoAsync("发布失败");
        }
        finally
        {
            _isPublishing = false;
        }
    }
}