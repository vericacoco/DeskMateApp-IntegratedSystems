using System.Collections.Generic;
using System.Reflection.Emit;
using DeskMateApp.Domain.DomainModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using DeskMateApp.Domain.Identity;

namespace DeskMateApp.Repository.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
    {
    }

    public DbSet<OfficeLocation> OfficeLocations { get; set; }
    public DbSet<Desk> Desks { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Amenity> Amenities { get; set; }
    public DbSet<DeskAmenity> DeskAmenities { get; set; }
   

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Desk -> Location (1..many)
        builder.Entity<Desk>()
            .HasOne(d => d.OfficeLocation)
            .WithMany(l => l.Desks)
            .HasForeignKey(d => d.OfficeLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // Reservation -> Desk (1..many)
        builder.Entity<Reservation>()
            .HasOne(r => r.Desk)
            .WithMany(d => d.Reservations)
            .HasForeignKey(r => r.DeskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Reservation>()
             .HasOne(r => r.User)
             .WithMany()              
             .HasForeignKey(r => r.UserId)
             .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Reservation>()
            .Property(r => r.UserId)
            .IsRequired();

        // DeskAmenity (join) - Desk
        builder.Entity<DeskAmenity>()
            .HasOne(da => da.Desk)
            .WithMany(d => d.DeskAmenities)
            .HasForeignKey(da => da.DeskId)
            .OnDelete(DeleteBehavior.Restrict);

        // DeskAmenity (join) - Amenity
        builder.Entity<DeskAmenity>()
            .HasOne(da => da.Amenity)
            .WithMany(a => a.DeskAmenities)
            .HasForeignKey(da => da.AmenityId)
            .OnDelete(DeleteBehavior.Restrict);

        // Optional: уникатност (да нема дупликат Amenity на исто Desk)
        builder.Entity<DeskAmenity>()
            .HasIndex(da => new { da.DeskId, da.AmenityId })
            .IsUnique();
    }
}
