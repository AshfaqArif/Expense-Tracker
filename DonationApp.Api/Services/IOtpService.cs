namespace DonationApp.Api.Services;

public interface IOtpService
{
    string GenerateOtp();
    bool ValidateOtpFormat(string otp);
}

