namespace JiuLing.Platform.Models;
public class AnalysisResultDto(
    string fileName,
    int size,
    string sha1,
    string md5,
    int analysisDate,
    PlatformAnalysisStats platformStats,
    List<PlatformAnalysisResults> platformResults,
    int reputation,
    string? knownDistributor)
{
    public string FileName { get; set; } = fileName;
    public int Size { get; set; } = size;
    public string SHA1 { get; set; } = sha1;
    public string MD5 { get; set; } = md5;
    public int AnalysisDate { get; set; } = analysisDate;
    public PlatformAnalysisStats PlatformStats { get; set; } = platformStats;
    public List<PlatformAnalysisResults> PlatformResults { get; set; } = platformResults;
    public int Reputation { get; set; } = reputation;
    public string? KnownDistributor { get; set; } = knownDistributor;
}

public class PlatformAnalysisStats(int malicious, int undetected)
{
    public int Malicious { get; set; } = malicious;
    public int Undetected { get; set; } = undetected;
}

public class PlatformAnalysisResults(string category, string engineName, string result)
{
    public string Category { get; set; } = category;
    public string EngineName { get; set; } = engineName;
    public string Result { get; set; } = result;

    public int Sort
    {
        get
        {
            return Category switch
            {
                "malicious" => 1,
                "undetected" => 2,
                "type-unsupported" => 3,
                _ => 99,
            };
        }
    }
}