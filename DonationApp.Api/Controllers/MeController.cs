using System.Security.Claims;
using DonationApp.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DonationApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Donor")]
public class MeController : ControllerBase
{
    private readonly AppDbContext _db;
    public MeController(AppDbContext db)
    {
        _db = db;
    }

    private Guid GetCurrentDonorId()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.Parse(id!);
    }

    [HttpGet("overview")]
    public async Task<IActionResult> Overview()
    {
        var donorId = GetCurrentDonorId();
        var donor = await _db.Donors.FirstAsync(d => d.Id == donorId);
        var donations = await _db.Donations.Where(d => d.DonorId == donorId).OrderByDescending(d => d.DateOfDonation).ToListAsync();
        var offerings = await _db.DonationOfferings.Include(o => o.Purpose).Where(o => o.DonorId == donorId).ToListAsync();
        var subscriptions = await _db.Subscriptions.Where(s => s.DonorId == donorId).ToListAsync();
        return Ok(new { donor, donations, offerings, subscriptions });
    }
}

