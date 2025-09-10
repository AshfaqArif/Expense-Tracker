using System.ComponentModel.DataAnnotations;
using DonationApp.Api.Data;
using DonationApp.Api.Models;
using DonationApp.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DonationApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IOtpService _otpService;
    private readonly ITokenService _tokenService;

    // Simple in-memory store for OTPs with TTL for demo purposes
    private static readonly Dictionary<string, (string Otp, DateTime ExpiresAt)> _otpStore = new();

    public AuthController(AppDbContext db, IOtpService otpService, ITokenService tokenService)
    {
        _db = db;
        _otpService = otpService;
        _tokenService = tokenService;
    }

    public class RegisterRequest
    {
        [Required]
        [RegularExpression("^[A-Za-z ]+$")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^[0-9]{6,15}$")]
        public string ContactNumber { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^[A-Za-z ]+$")]
        public string State { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^[A-Za-z ]+$")]
        public string District { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^[0-9]{6}$")]
        public string Pincode { get; set; } = string.Empty;
    }

    [HttpPost("register")] 
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var exists = await _db.Donors.AnyAsync(d => d.ContactNumber == request.ContactNumber);
        if (exists)
        {
            return Conflict(new { message = "Contact number already registered" });
        }

        var donor = new Donor
        {
            Name = request.Name.Trim(),
            ContactNumber = request.ContactNumber,
            State = request.State.Trim(),
            District = request.District.Trim(),
            Pincode = request.Pincode
        };
        _db.Donors.Add(donor);
        await _db.SaveChangesAsync();

        // Send OTP (here we just return it for demo; in real app, send via SMS)
        var otp = _otpService.GenerateOtp();
        _otpStore[donor.ContactNumber] = (otp, DateTime.UtcNow.AddMinutes(5));

        return Ok(new { message = "Registered successfully. Verify OTP to login.", otp });
    }

    public class RequestOtpRequest
    {
        [Required]
        [RegularExpression("^[0-9]{6,15}$")]
        public string ContactNumber { get; set; } = string.Empty;
    }

    [HttpPost("request-otp")]
    public async Task<IActionResult> RequestOtp([FromBody] RequestOtpRequest request)
    {
        var donor = await _db.Donors.FirstOrDefaultAsync(d => d.ContactNumber == request.ContactNumber);
        if (donor == null)
        {
            return NotFound(new { message = "Donor not found" });
        }
        var otp = _otpService.GenerateOtp();
        _otpStore[donor.ContactNumber] = (otp, DateTime.UtcNow.AddMinutes(5));
        return Ok(new { message = "OTP generated.", otp });
    }

    public class VerifyOtpRequest
    {
        [Required]
        [RegularExpression("^[0-9]{6,15}$")]
        public string ContactNumber { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^[0-9]{6}$")]
        public string Otp { get; set; } = string.Empty;
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        if (!_otpService.ValidateOtpFormat(request.Otp))
        {
            return BadRequest(new { message = "OTP must be a 6 digit number." });
        }

        if (!_otpStore.TryGetValue(request.ContactNumber, out var entry) || entry.ExpiresAt < DateTime.UtcNow)
        {
            return BadRequest(new { message = "OTP expired or not requested." });
        }

        if (!string.Equals(entry.Otp, request.Otp, StringComparison.Ordinal))
        {
            return BadRequest(new { message = "Invalid OTP." });
        }

        var donor = await _db.Donors.FirstOrDefaultAsync(d => d.ContactNumber == request.ContactNumber);
        if (donor == null)
        {
            return NotFound(new { message = "Donor not found" });
        }

        var token = _tokenService.GenerateTokenForDonor(donor);
        _otpStore.Remove(request.ContactNumber);
        return Ok(new { token });
    }
}

