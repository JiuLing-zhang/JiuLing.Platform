using System.Collections.Concurrent;
using System.Text.Json;
using JiuLing.Platform.Components.Common;
using JiuLing.Platform.Services.HashCheckService;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using MudBlazor;

namespace JiuLing.Platform.Components.Pages.Tools;
public partial class FileTransferSender(
    IOptions<AppSettings> options,
    IJSRuntime jsRuntime,
    IDialogService dialog,
    HashServiceFactory hashServiceFactory,
    NavigationManager navigation)
{
    private readonly AppSettings _appSettings = options.Value;

    private bool _isClosePage;
    private int _roomId;
    private bool _isLoading;
    private bool _isRoomCreated;
    private string _loadingMessage = "";
    private string _qrValue = "";

    private bool _isReceiverJoined;
    private ConnectionTypeEnum _connectionType = ConnectionTypeEnum.None;

    private HubConnection _hub = null!;
    private DotNetObjectReference<FileTransferSender>? _objRef;

    private readonly List<FileTransferInfo> _files = [];
    private readonly SemaphoreSlim _fileQueueSlim = new(1, 1);
    private readonly ConcurrentQueue<FileTransferInfo> _fileQueue = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
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
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        Console.WriteLine("准备初始化房间....");
        _objRef = DotNetObjectReference.Create(this);

        _hub = new HubConnectionBuilder()
            .WithUrl($"{navigation.BaseUri}file-transfer-hub")
            .Build();


        _hub.On("ReceiverJoin", async () =>
        {
            Console.WriteLine("接收方进入");
            _isReceiverJoined = true;
            await InvokeAsync(StateHasChanged);
            await jsRuntime.InvokeVoidAsync("createSenderConnection");
        });

        _hub.On<string>("ReceiveReceiverIceCandidate", async (candidate) =>
        {
            Console.WriteLine("收到接收方候选人信息");
            await InvokeAsync(StateHasChanged);
            await jsRuntime.InvokeVoidAsync("receiveIceCandidate", candidate);
        });

        _hub.On<string>("ReceiveAnswer", async (answer) =>
        {
            Console.WriteLine("收到网络通道响应指令");
            await jsRuntime.InvokeVoidAsync("receiveAnswer", answer);
            await InvokeAsync(StateHasChanged);
        });
        await _hub.StartAsync();

        await jsRuntime.InvokeVoidAsync("initialization", _objRef, _appSettings.StunServer);

        _roomId = await _hub.InvokeAsync<int>("CreateRoom");
        _qrValue = $"{navigation.BaseUri}file-transfer/receiver/{_roomId}";

        Console.WriteLine("等待接收方进入....");
        await Task.Factory.StartNew(StartSendFileQueueAsync, TaskCreationOptions.LongRunning);

        _isRoomCreated = true;
    }

    [JSInvokable]
    public async Task SendIceCandidateToServer(string candidate)
    {
        Console.WriteLine("准备发送候选人信息....");
        var result = await _hub.InvokeAsync<string>("SendSenderIceCandidate", candidate);
        Console.WriteLine($"服务器返回:{result}");
    }

    [JSInvokable]
    public async Task SendOfferToServer(string offer)
    {
        Console.WriteLine("准备发送网络通道请求指令....");
        var result = await _hub.InvokeAsync<string>("SendOffer", offer);
        Console.WriteLine($"服务器返回:{result}");
    }

    [JSInvokable]
    public async Task SenderConnected()
    {
        //发送端准备就绪
        _connectionType = ConnectionTypeEnum.WebRTC;
        await InvokeAsync(StateHasChanged);
    }

    public async Task EnableServiceRelay()
    {
        _connectionType = ConnectionTypeEnum.ServiceRelay;
        var result = await _hub.InvokeAsync<string>("SwitchConnectionType");
        Console.WriteLine($"服务器返回:{result}");
        await InvokeAsync(StateHasChanged);
    }

    private async Task UploadFiles(IReadOnlyList<IBrowserFile> browserFiles)
    {
        long maxFileSize = 100000000;
        IList<IBrowserFile> files = new List<IBrowserFile>();
        foreach (var browserFile in browserFiles)
        {
            if (browserFile.Size >= maxFileSize)
            {
                await dialog.ShowInfoAsync("文件大小超过系统限制");
                return;
            }

            if (_files.Any(x => x.FileName == browserFile.Name))
            {
                await dialog.ShowInfoAsync("不能重复添加文件");
                return;
            }

            files.Add(browserFile);
        }

        await LoadingAsync("正在处理文件...");

        foreach (var browserFile in files)
        {
            var file = new FileTransferInfo
            {
                FileName = browserFile.Name
            };
            var ms = new MemoryStream();
            await browserFile.OpenReadStream(maxFileSize).CopyToAsync(ms);
            var buffer = ms.ToArray();
            file.FileContext = new List<byte>(buffer);
            file.FileSize = buffer.Length;
            var hashService = hashServiceFactory.Create(HashTypeEnum.SHA1);
            file.SHA1 = await hashService.ComputeHashAsync(buffer, false);

            _files.Add(file);
        }
        await LoadingCompletedAsync();
    }

    private async Task SendFileAsync(string fileName)
    {
        var file = _files.First(x => x.FileName == fileName);
        if (file.State != FileTransferStateEnum.Init)
        {
            return;
        }
        file.State = FileTransferStateEnum.Queue;
        _fileQueue.Enqueue(file);
        await InvokeAsync(StateHasChanged);
    }

    private async Task SendAllFilesAsync()
    {
        foreach (var file in _files)
        {
            if (file.State != FileTransferStateEnum.Init)
            {
                continue;
            }
            file.State = FileTransferStateEnum.Queue;
            _fileQueue.Enqueue(file);
        }
        await InvokeAsync(StateHasChanged);
    }

    private async Task StartSendFileQueueAsync()
    {
        while (!_isClosePage)
        {
            while (_fileQueue.TryDequeue(out var file))
            {
                await _fileQueueSlim.WaitAsync();
                _files.First(x => x.FileName == file.FileName).State = FileTransferStateEnum.Sending;
                var fileMetadata = file as FileMetadata;
                if (_connectionType == ConnectionTypeEnum.WebRTC)
                {
                    await jsRuntime.InvokeVoidAsync("sendFileInfo", JsonSerializer.Serialize(fileMetadata));
                    await jsRuntime.InvokeVoidAsync("sendFile", file.FileContext.ToArray());
                }
                else if (_connectionType == ConnectionTypeEnum.ServiceRelay)
                {
                    await _hub.InvokeAsync("SendFileInfo", JsonSerializer.Serialize(fileMetadata));
                    await SendFileWithSignalRAsync();
                }
                await InvokeAsync(StateHasChanged);
                await Task.Delay(100);
            }
            await Task.Delay(1000);
        }
    }

    private readonly int _chunkSize = 16384;
    private async Task SendFileWithSignalRAsync()
    {
        var file = _files.First(x => x.State == FileTransferStateEnum.Sending);
        int totalBytesSent = 0;

        for (int offset = 0; offset < file.FileContext.Count; offset += _chunkSize)
        {
            int remainingBytes = file.FileContext.Count - offset;
            int chunkToSend = Math.Min(_chunkSize, remainingBytes);
            byte[] chunk = new byte[chunkToSend];
            file.FileContext.CopyTo(offset, chunk, 0, chunkToSend);

            await _hub.InvokeAsync("SendFile", chunk);

            totalBytesSent += chunkToSend;
            file.Progress = (double)totalBytesSent / file.FileContext.Count * 100;

            await InvokeAsync(StateHasChanged);
            await Task.Delay(20);
        }
        await _hub.InvokeAsync("SendFileSent");
        file.State = FileTransferStateEnum.Sent;
        _fileQueueSlim.Release();
    }

    [JSInvokable]
    public async Task FileSending(int length)
    {
        var file = _files.First(x => x.State == FileTransferStateEnum.Sending);
        file.Progress = (double)(file.FileSize - length) / file.FileSize * 100;
        await InvokeAsync(StateHasChanged);
    }

    [JSInvokable]
    public async Task FileSent()
    {
        _files.First(x => x.State == FileTransferStateEnum.Sending).State = FileTransferStateEnum.Sent;
        _fileQueueSlim.Release();
        await InvokeAsync(StateHasChanged);
    }

    private async Task LoadingAsync(string message)
    {
        _isLoading = true;
        _loadingMessage = message;
        await InvokeAsync(StateHasChanged);
    }
    private async Task LoadingCompletedAsync()
    {
        _isLoading = false;
        _loadingMessage = "";
        await InvokeAsync(StateHasChanged);
    }

    public void Dispose()
    {
        _isClosePage = true;
        _objRef?.Dispose();
    }
}