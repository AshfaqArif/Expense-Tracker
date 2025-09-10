using System.ComponentModel.DataAnnotations;
using DonationApp.Api.Data;
using DonationApp.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DonationApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AdminController(AppDbContext db, ITokenService tokenService, IConfiguration configuration)
    {
        _db = db;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public class AdminLoginRequest
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] AdminLoginRequest request)
    {
        var adminUser = _configuration.GetValue<string>("Admin:Username") ?? "admin";
        var adminPass = _configuration.GetValue<string>("Admin:Password") ?? "admin123";
        if (request.Username == adminUser && request.Password == adminPass)
        {
            var token = _tokenService.GenerateTokenForAdmin(request.Username);
            return Ok(new { token });
        }
        return Unauthorized(new { message = "Invalid admin credentials" });
    }

    [HttpGet("donors")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetDonors()
    {
        var donors = await _db.Donors.OrderByDescending(d => d.CreatedAt).ToListAsync();
        return Ok(donors);
    }

    [HttpGet("donations")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllDonations()
    {
        var items = await _db.Donations.Include(d => d.Donor).OrderByDescending(d => d.DateOfDonation).ToListAsync();
        return Ok(items);
    }
}

