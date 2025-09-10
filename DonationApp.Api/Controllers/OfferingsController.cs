using System.Security.Claims;
using DonationApp.Api.Data;
using DonationApp.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DonationApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OfferingsController : ControllerBase
{
    private readonly AppDbContext _db;
    public OfferingsController(AppDbContext db)
    {
        _db = db;
    }

    private Guid GetCurrentDonorId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(id!);
    }

    [HttpGet("purposes")]
    public async Task<IActionResult> GetPurposes()
    {
        var list = await _db.OfferingPurposes.OrderBy(p => p.Name).ToListAsync();
        return Ok(list);
    }

    [HttpPost]
    [Authorize(Roles = "Donor")]
    public async Task<IActionResult> Create([FromBody] DonationOffering offering)
    {
        offering.Id = Guid.NewGuid();
        offering.DonorId = GetCurrentDonorId();
        _db.DonationOfferings.Add(offering);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMine), new { id = offering.Id }, offering);
    }

    [HttpGet]
    [Authorize(Roles = "Donor")]
    public async Task<IActionResult> GetMine()
    {
        var donorId = GetCurrentDonorId();
        var list = await _db.DonationOfferings.Include(o => o.Purpose)
            .Where(o => o.DonorId == donorId).ToListAsync();
        return Ok(list);
    }
}

