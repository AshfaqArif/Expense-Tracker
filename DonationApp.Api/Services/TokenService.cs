using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DonationApp.Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DonationApp.Api.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateTokenForDonor(Donor donor)
    {
        return GenerateToken(new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, donor.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, donor.Id.ToString()),
            new Claim(ClaimTypes.Name, donor.Name),
            new Claim(ClaimTypes.MobilePhone, donor.ContactNumber),
            new Claim(ClaimTypes.Role, "Donor"),
        });
    }

    public string GenerateTokenForAdmin(string adminName)
    {
        return GenerateToken(new List<Claim>
        {
            new Claim(ClaimTypes.Name, adminName),
            new Claim(ClaimTypes.Role, "Admin"),
        });
    }

    private string GenerateToken(IEnumerable<Claim> claims)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(jwtSection.GetValue<int>("ExpiresMinutes"));

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

