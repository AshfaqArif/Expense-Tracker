using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DonationApp.Api.Models;

public class DonationOffering
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid DonorId { get; set; }

    [ForeignKey(nameof(DonorId))]
    public Donor? Donor { get; set; }

    [Required]
    public int PurposeId { get; set; }

    [ForeignKey(nameof(PurposeId))]
    public OfferingPurpose? Purpose { get; set; }

    [Required]
    [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
    public decimal Amount { get; set; }

    [Required]
    public string Frequency { get; set; } = "Weekly"; // Weekly or Monthly
}

