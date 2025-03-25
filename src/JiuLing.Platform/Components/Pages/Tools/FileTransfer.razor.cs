namespace JiuLing.Platform.Components.Pages.Tools;

public partial class FileTransfer(NavigationManager navigation)
{
    private Task GotoSenderPageAsync()
    {
        navigation.NavigateTo($"/file-transfer/sender");
        return Task.CompletedTask;
    }

    private Task InputRoomIdChanged(string? value)
    {
        value ??= "";
        if (value.Length == 4 && int.TryParse(value, out var roomId))
        {
            navigation.NavigateTo($"/file-transfer/receiver/{roomId}");
        }
        return Task.CompletedTask;
    }
}