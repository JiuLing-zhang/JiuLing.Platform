using JiuLing.Platform.Common.Enums;

namespace JiuLing.Platform.Models;

public class FileTransferInfo : FileMetadata
{
    public List<byte> FileContext { get; set; }
    public FileTransferStateEnum State { get; set; }
    public double Progress { get; set; }
    public bool Succeed { get; set; }
    public string Message { get; set; }
}

public class FileMetadata
{
    public string FileName { get; set; } = null!;
    public string SHA1 { get; set; } = null!;
    public int FileSize { get; set; }
}