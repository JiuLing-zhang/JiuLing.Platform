namespace JiuLing.Platform.Models.Entities;
[Table("ConfigBase")]
public class ConfigBase
{
    [Key]
    public string ConfigKey { get; set; } = null!;

    public string ConfigDetail { get; set; } = null!;
}