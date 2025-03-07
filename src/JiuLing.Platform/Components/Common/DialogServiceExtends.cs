using MudBlazor;

namespace JiuLing.Platform.Components.Common;

public static class DialogServiceExtends
{
    public static async Task<IDialogReference> ShowInfoAsync(this IDialogService dialogService, string text)
    {
        var parameters = new DialogParameters { { "ContentText", text } };
        return await dialogService.ShowAsync<DialogOk>("提示", parameters);
    }

    private static readonly DialogOptions MessageBoxOptions = new()
    {
        BackdropClick = false
    };

    public static async Task<bool?> ShowMessageBoxAsync(this IDialogService dialogService, string message)
    {
        return await dialogService.ShowMessageBox("提示", message, yesText: "确定", options: MessageBoxOptions);
    }

    public static async Task<bool?> ShowYesOrNoAsync(this IDialogService dialogService, string message, string yesText)
    {
        return await dialogService.ShowMessageBox("提示", message, yesText, noText: "取消");
    }
}