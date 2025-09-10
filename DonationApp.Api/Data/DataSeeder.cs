using DonationApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DonationApp.Api.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        await db.Database.MigrateAsync(cancellationToken);

        if (!await db.OfferingPurposes.AnyAsync(cancellationToken))
        {
            db.OfferingPurposes.AddRange(new[]
            {
                new OfferingPurpose { Name = "Education" },
                new OfferingPurpose { Name = "Health" },
                new OfferingPurpose { Name = "Food" },
                new OfferingPurpose { Name = "Shelter" }
            });
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}

