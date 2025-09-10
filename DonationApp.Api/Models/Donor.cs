using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DonationApp.Api.Models;

public class Donor
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(100)]
    [RegularExpression("^[A-Za-z ]+$", ErrorMessage = "Name must contain only alphabets and spaces.")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    [RegularExpression("^[0-9]{6,15}$", ErrorMessage = "Contact number must be 6-15 digits.")]
    public string ContactNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [RegularExpression("^[A-Za-z ]+$", ErrorMessage = "State must contain only alphabets and spaces.")]
    public string State { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    [RegularExpression("^[A-Za-z ]+$", ErrorMessage = "District must contain only alphabets and spaces.")]
    public string District { get; set; } = string.Empty;

    [Required]
    [MaxLength(6)]
    [RegularExpression("^[0-9]{6}$", ErrorMessage = "Pincode must be a 6 digit number.")]
    public string Pincode { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }
        = null;
    public DateTimeOffset? LastLoginAt { get; set; }
        = null;

    public ICollection<Donation> Donations { get; set; } = new List<Donation>();
    public ICollection<DonationOffering> DonationOfferings { get; set; } = new List<DonationOffering>();
    public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}

