namespace JiuLing.Platform.Services;
public interface IVirusTotalService
{
    Task<(bool Succeed, string Message, AnalysisResultDto? Dto)> CheckByFileHashAsync(string sha1);
}