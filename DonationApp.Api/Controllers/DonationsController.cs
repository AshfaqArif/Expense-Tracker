using System.Security.Claims;
using DonationApp.Api.Data;
using DonationApp.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DonationApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DonationsController : ControllerBase
{
    private readonly AppDbContext _db;

    public DonationsController(AppDbContext db)
    {
        _db = db;
    }

    private Guid GetCurrentDonorId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(id!);
    }

    [HttpPost]
    [Authorize(Roles = "Donor")]
    public async Task<IActionResult> Create([FromBody] Donation donation)
    {
        donation.Id = Guid.NewGuid();
        donation.DonorId = GetCurrentDonorId();
        if (donation.DateOfDonation == default)
        {
            donation.DateOfDonation = DateOnly.FromDateTime(DateTime.UtcNow);
        }
        _db.Donations.Add(donation);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = donation.Id }, donation);
    }

    [HttpGet]
    [Authorize(Roles = "Donor")]
    public async Task<IActionResult> GetMine()
    {
        var donorId = GetCurrentDonorId();
        var list = await _db.Donations.Where(d => d.DonorId == donorId).OrderByDescending(d => d.DateOfDonation).ToListAsync();
        return Ok(list);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Donor,Admin")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var donorIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isAdmin = User.IsInRole("Admin");
        var entity = await _db.Donations.FindAsync(id);
        if (entity == null) return NotFound();
        if (!isAdmin && entity.DonorId.ToString() != donorIdClaim) return Forbid();
        return Ok(entity);
    }
}

