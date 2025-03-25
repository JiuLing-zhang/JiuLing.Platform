using System.ComponentModel;

namespace JiuLing.Platform.Common.Enums;
public enum IssueStatusEnum
{
    [Description("已关闭")]
    Close = 0,
    [Description("进行中")]
    Open = 1
}