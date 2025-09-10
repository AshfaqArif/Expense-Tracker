using System.Text.RegularExpressions;

namespace DonationApp.Api.Services;

public class OtpService : IOtpService
{
    private static readonly Regex OtpRegex = new Regex("^[0-9]{6}$", RegexOptions.Compiled);
    private readonly Random _random = new Random();

    public string GenerateOtp()
    {
        return _random.Next(0, 1000000).ToString("D6");
    }

    public bool ValidateOtpFormat(string otp)
    {
        return OtpRegex.IsMatch(otp);
    }
}

