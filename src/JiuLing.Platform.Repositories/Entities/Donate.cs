﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JiuLing.Platform.Repositories.Entities;

[Table("Donation")]
public class Donation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string User { get; set; }
    public DateTime Time { get; set; }
    public decimal Amount { get; set; }
    public string? Message { get; set; }
    public string? Note { get; set; }
}