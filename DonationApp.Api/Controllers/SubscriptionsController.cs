using System.Security.Claims;
using DonationApp.Api.Data;
using DonationApp.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DonationApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly AppDbContext _db;
    public SubscriptionsController(AppDbContext db)
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
    public async Task<IActionResult> Create([FromBody] Subscription subscription)
    {
        subscription.Id = Guid.NewGuid();
        subscription.DonorId = GetCurrentDonorId();
        if (subscription.StartDate == default)
        {
            subscription.StartDate = DateOnly.FromDateTime(DateTime.UtcNow);
        }
        _db.Subscriptions.Add(subscription);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetMine), new { id = subscription.Id }, subscription);
    }

    [HttpGet]
    [Authorize(Roles = "Donor")]
    public async Task<IActionResult> GetMine()
    {
        var donorId = GetCurrentDonorId();
        var list = await _db.Subscriptions.Where(s => s.DonorId == donorId).ToListAsync();
        return Ok(list);
    }
}

