using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DonationApp.Api.Models;

public class Subscription
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid DonorId { get; set; }

    [ForeignKey(nameof(DonorId))]
    public Donor? Donor { get; set; }

    [Required]
    [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    [Required]
    [RegularExpression("^(Monthly|Yearly)$", ErrorMessage = "Frequency must be Monthly or Yearly.")]
    public string Frequency { get; set; } = "Monthly"; // Monthly or Yearly

    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
}

