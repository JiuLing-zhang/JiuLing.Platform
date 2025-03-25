namespace JiuLing.Platform.Common;

public class PagedResult<T>
{
    public List<T> Items { get; set; } = null!;
    public int TotalCount { get; set; }
}
