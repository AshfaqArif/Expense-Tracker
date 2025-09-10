using System.ComponentModel.DataAnnotations;

namespace DonationApp.Api.Models;

public class OfferingPurpose
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}

