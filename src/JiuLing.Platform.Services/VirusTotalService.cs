using System.Reflection;
using JiuLing.CommonLibs.ExtensionMethods;

namespace JiuLing.Platform.Services;

public class VirusTotalService(IHttpClientFactory httpClientFactory) : IVirusTotalService
{
    public async Task<(bool Succeed, string Message, AnalysisResultDto? Dto)> CheckByFileHashAsync(string sha1)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/api/v3/files/{sha1}");
        var response = await httpClientFactory.CreateClient("VirusTotal").SendAsync(requestMessage);
        var responseBody = await response.Content.ReadAsStringAsync();
        var vtResult = responseBody.ToObject<AnalysisHttpResponse>();
        if (vtResult == null)
        {
            return (false, "连接服务器失败", null);
        }
        if (vtResult.error != null)
        {
            return (false, vtResult.error.message, null);
        }

        if (vtResult.data == null)
        {
            return (false, "服务器数据格式异常", null);
        }

        var vtAttributes = vtResult.data.attributes;
        //不同引擎的分析结果
        var platformAnalysisResults = new List<PlatformAnalysisResults>();
        var lastAnalysisResults = vtAttributes.last_analysis_results;
        PropertyInfo[] properties = lastAnalysisResults.GetType().GetProperties();
        foreach (PropertyInfo property in properties)
        {
            var engineDetail = property.GetValue(lastAnalysisResults, null) as AnalysisResults;
            if (engineDetail == null)
            {
                continue;
            }
            var platformResult = engineDetail.result;
            if (platformResult == "null")
            {
                platformResult = "";
            }
            platformAnalysisResults.Add(new PlatformAnalysisResults(engineDetail.category, engineDetail.engine_name, platformResult));
        }

        //不同引擎的状态汇总
        var platformAnalysisStats = new PlatformAnalysisStats(
            vtAttributes.last_analysis_stats.malicious,
            vtAttributes.last_analysis_stats.undetected);

        //是否为已知的软件提供商
        string? knownDistributor = null;
        if (vtAttributes.known_distributors != null)
        {
            knownDistributor = vtAttributes.known_distributors.distributors.FirstOrDefault();
        }

        var result = new AnalysisResultDto(
            vtAttributes.meaningful_name,
            vtAttributes.size,
            vtAttributes.sha1,
            vtAttributes.md5,
            vtAttributes.last_analysis_date,
            platformAnalysisStats,
            platformAnalysisResults,
            vtAttributes.reputation,
            knownDistributor);
        return (true, "", result);
    }
}