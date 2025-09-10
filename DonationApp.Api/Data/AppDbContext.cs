using DonationApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DonationApp.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Donor> Donors => Set<Donor>();
    public DbSet<Donation> Donations => Set<Donation>();
    public DbSet<DonationOffering> DonationOfferings => Set<DonationOffering>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<OfferingPurpose> OfferingPurposes => Set<OfferingPurpose>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Donor>()
            .HasIndex(d => d.ContactNumber)
            .IsUnique();

        modelBuilder.Entity<Donation>()
            .HasOne(d => d.Donor)
            .WithMany(p => p.Donations)
            .HasForeignKey(d => d.DonorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DonationOffering>()
            .HasOne(d => d.Donor)
            .WithMany(p => p.DonationOfferings)
            .HasForeignKey(d => d.DonorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DonationOffering>()
            .HasOne(d => d.Purpose)
            .WithMany()
            .HasForeignKey(d => d.PurposeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Subscription>()
            .HasOne(s => s.Donor)
            .WithMany(p => p.Subscriptions)
            .HasForeignKey(s => s.DonorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

