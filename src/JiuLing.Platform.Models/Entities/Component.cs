﻿namespace JiuLing.Platform.Models.Entities;
[Table("Component")]
public class Component
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Icon { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string GitHub { get; set; } = null!;
}