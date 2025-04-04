﻿using JiuLing.Platform.Components.Common;
using JiuLing.Platform.Services.HashCheckService;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MudBlazor;

namespace JiuLing.Platform.Components.Pages.Tools;

public partial class FileTransferReceiver(
    IOptions<AppSettings> options,
    IJSRuntime jsRuntime,
    NavigationManager navigation,
    IDialogService dialogService,
    HashServiceFactory hashServiceFactory)
{
    private readonly AppSettings _appSettings = options.Value;

    [Parameter]
    public int RoomId { get; set; }


    private ConnectionTypeEnum _connectionType = ConnectionTypeEnum.None;

    private HubConnection _hub = null!;
    private DotNetObjectReference<FileTransferReceiver> _objRef = null!;

    private readonly List<FileTransferInfo> _files = new List<FileTransferInfo>();

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        if (!RendererInfo.IsInteractive)
        {
            return;
        }

        await jsRuntime.InvokeVoidAsync("eval", @"
                var script1 = document.createElement('script');
                script1.src = '/js/file-transfer.js';
                document.head.appendChild(script1);

                var script2 = document.createElement('script');
                script2.src = '/js/crypto-js.js';
                document.head.appendChild(script2);

                var script3 = document.createElement('script');
                script3.src = '/js/crypto-js-inner.js';
                document.head.appendChild(script3);
            ");

        Console.WriteLine($"准备初始化模块....");
        _objRef = DotNetObjectReference.Create(this);

        _hub = new HubConnectionBuilder()
            .WithUrl($"{navigation.BaseUri}file-transfer-hub")
            .Build();

        _hub.On<string>("ReceiveSenderIceCandidate", async (candidate) =>
        {
            Console.WriteLine("收到发送方候选人信息");
            await InvokeAsync(StateHasChanged);
            await jsRuntime.InvokeVoidAsync("receiveIceCandidate", candidate);
        });

        _hub.On<string>("ReceiveOffer", async (offer) =>
        {
            Console.WriteLine("收到网络通道请求指令");
            await InvokeAsync(StateHasChanged);
            await jsRuntime.InvokeVoidAsync("createReceiverConnection", offer);
        });
        _hub.On("ReceiveSwitchConnectionType", async () =>
        {
            //切换服务器中转模式
            _connectionType = ConnectionTypeEnum.ServiceRelay;
            await InvokeAsync(StateHasChanged);
        });
        _hub.On<string>("ReceiveFileInfo", async (fileInfo) =>
        {
            await OnReceiveFileInfo(fileInfo);
        });
        _hub.On<byte[]>("ReceiveFile", async (buffer) =>
        {
            await OnFileReceivingAsync(buffer);
        });
        _hub.On("ReceiveFileSent", async () =>
        {
            await OnFileReceived();
        });

        await _hub.StartAsync();
        await jsRuntime.InvokeVoidAsync("initialization", _objRef, _appSettings.StunServer);

        var result = await _hub.InvokeAsync<string>("JoinRoom", RoomId);
        if (result != "ok")
        {
            await dialogService.ShowMessageBoxAsync(result);
            navigation.NavigateTo($"/file-transfer");
        }
    }

    [JSInvokable]
    public async Task SendIceCandidateToServer(string candidate)
    {
        Console.WriteLine("准备发送候选人信息....");
        var result = await _hub.InvokeAsync<string>("SendReceiverIceCandidate", candidate);
        Console.WriteLine($"服务器返回:{result}");
    }

    [JSInvokable]
    public async Task SendAnswerToServer(string answer)
    {
        Console.WriteLine("准备发送网络通道响应指令....");
        var result = await _hub.InvokeAsync<string>("SendAnswer", answer);
        Console.WriteLine($"服务器返回:{result}");
        await InvokeAsync(StateHasChanged);
    }

    [JSInvokable]
    public async Task ReceiverConnected()
    {
        //接收端准备就绪
        _connectionType = ConnectionTypeEnum.WebRTC;
        await InvokeAsync(StateHasChanged);
    }

    [JSInvokable]
    public async Task FileReceivingWithWebRTC(byte[] buffer)
    {
        await OnFileReceivingAsync(buffer);
    }

    [JSInvokable]
    public async Task FileInfoReceived(string fileInfo)
    {
        await OnReceiveFileInfo(fileInfo);
    }

    private async Task OnReceiveFileInfo(string fileInfo)
    {
        var file = fileInfo.ToObject<FileTransferInfo>();
        if (file == null)
        {
            return;
        }
        file.FileContext = new List<byte>();
        file.State = FileTransferStateEnum.Sending;
        _files.Add(file);
        await InvokeAsync(StateHasChanged);
    }

    [JSInvokable]
    public async Task FileReceivedWithWebRTC()
    {
        await OnFileReceived();
    }

    private async Task OnFileReceivingAsync(byte[] buffer)
    {
        var file = _files.First(x => x.State == FileTransferStateEnum.Sending);
        file.FileContext.AddRange(buffer);
        file.Progress = (double)file.FileContext.Count / file.FileSize * 100;
        await InvokeAsync(StateHasChanged);
    }

    private async Task OnFileReceived()
    {
        var file = _files.First(x => x.State == FileTransferStateEnum.Sending);
        var sha1 = await hashServiceFactory.Create(HashTypeEnum.SHA1).ComputeHashAsync(file.FileContext.ToArray(), false);
        if (file.SHA1 != sha1)
        {
            file.Message = "文件校验失败";
            file.Succeed = false;
        }
        else
        {
            file.Succeed = true;
        }
        file.State = FileTransferStateEnum.Sent;
        await InvokeAsync(StateHasChanged);
    }

    private async Task DownloadFileAsync(string fileName)
    {
        await jsRuntime.InvokeVoidAsync("saveToFileWithBufferAndName", fileName, _files.First(x => x.FileName == fileName).FileContext.ToArray());
    }

    public void Dispose()
    {
        _objRef?.Dispose();
    }
}