using System.ComponentModel;

namespace JiuLing.Platform.Common.Enums;
public enum IssueTypeEnum
{
    [Description("功能建议")]
    FeatureRequest = 1,

    [Description("错误报告")]
    BugReport = 2
}