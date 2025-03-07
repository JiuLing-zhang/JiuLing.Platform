using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiuLing.Platform.Models;

public class ConnectionDetail(int roomId, SenderInfo sender, ReceiverInfo? receiver)
{
    public int RoomId { get; set; } = roomId;
    public SenderInfo Sender { get; set; } = sender;
    public ReceiverInfo? Receiver { get; set; } = receiver;

    //半小时强制过期
    public DateTime ExpirationTime { get; set; } = DateTime.Now.AddMinutes(30);
}

public class SenderInfo(string id)
{
    public string Id { get; set; } = id;
    public string? Offer { get; set; }
    public string? Candidate { get; set; }
}

public class ReceiverInfo(string id)
{
    public string Id { get; set; } = id;
    public string? Answer { get; set; }
    public string? Candidate { get; set; }
}