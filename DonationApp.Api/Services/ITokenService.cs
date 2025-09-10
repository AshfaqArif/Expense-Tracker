using DonationApp.Api.Models;

namespace DonationApp.Api.Services;

public interface ITokenService
{
    string GenerateTokenForDonor(Donor donor);
    string GenerateTokenForAdmin(string adminName);
}

