using DeskMateApp.Repository.Data;
using DeskMateApp.Domain.DomainModels;
using DeskMateApp.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using DeskMateApp.Domain.Identity;

namespace DeskMateApp.Repository.Data;

public class ApplicationDbContextSeeder
{
    public static async Task SeedDatabase(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Roles
        string[] roles = { "Admin", "Employee" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Admin user
        var adminEmail = "admin@deskmate.com";
        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                FullName = "DeskMate Admin"
            };

            await userManager.CreateAsync(admin, "Admin123!");
        }

        if (!await userManager.IsInRoleAsync(admin, "Admin"))
            await userManager.AddToRoleAsync(admin, "Admin");

        // 1) Seed test user ако го нема
        var email = "testuser@deskmate.com";
        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = "Test User",
                Department = "IT",
                City = "Skopje"
            };

            await userManager.CreateAsync(user, "Test123!");
        }

        if (!await userManager.IsInRoleAsync(user, "Employee"))
            await userManager.AddToRoleAsync(user, "Employee");

        // 2) Seed само ако нема локации (за да не дуплира)
        if (context.OfficeLocations.Any()) return;

        // ---- Locations
        var loc1 = new OfficeLocation
        {
            Id = Guid.NewGuid(),
            Name = "HQ Skopje",
            City = "Skopje",
            Address = "Center"
        };

        var loc2 = new OfficeLocation
        {
            Id = Guid.NewGuid(),
            Name = "Office Bitola",
            City = "Bitola",
            Address = "Main street"
        };

        context.OfficeLocations.AddRange(loc1, loc2);

        // ---- Desks
        var desks = new List<Desk>
        {
            new Desk { Id = Guid.NewGuid(), Code = "D-01", Floor = 1, Type = DeskType.Standard, IsActive = true, OfficeLocationId = loc1.Id },
            new Desk { Id = Guid.NewGuid(), Code = "D-02", Floor = 1, Type = DeskType.Standing, IsActive = true, OfficeLocationId = loc1.Id },
            new Desk { Id = Guid.NewGuid(), Code = "D-03", Floor = 2, Type = DeskType.Standard, IsActive = true, OfficeLocationId = loc1.Id },

            new Desk { Id = Guid.NewGuid(), Code = "B-01", Floor = 1, Type = DeskType.Standard, IsActive = true, OfficeLocationId = loc2.Id },
            new Desk { Id = Guid.NewGuid(), Code = "B-02", Floor = 1, Type = DeskType.Standard, IsActive = true, OfficeLocationId = loc2.Id },
        };

        context.Desks.AddRange(desks);

        // ---- Amenities
        var monitor = new Amenity { Id = Guid.NewGuid(), Name = "Monitor" };
        var docking = new Amenity { Id = Guid.NewGuid(), Name = "Docking Station" };
        var window = new Amenity { Id = Guid.NewGuid(), Name = "Near Window" };
        var quiet = new Amenity { Id = Guid.NewGuid(), Name = "Quiet Zone" };

        context.Amenities.AddRange(monitor, docking, window, quiet);

        await context.SaveChangesAsync();

        // ---- DeskAmenities (join records)
        // D-01: Monitor + Docking
        // D-02: Standing + Near Window (amenity window)
        // D-03: Quiet Zone + Monitor
        // B-01: Docking
        // B-02: Near Window

        var deskAmenities = new List<DeskAmenity>
        {
            new DeskAmenity { Id = Guid.NewGuid(), DeskId = desks[0].Id, AmenityId = monitor.Id },
            new DeskAmenity { Id = Guid.NewGuid(), DeskId = desks[0].Id, AmenityId = docking.Id },

            new DeskAmenity { Id = Guid.NewGuid(), DeskId = desks[1].Id, AmenityId = window.Id },

            new DeskAmenity { Id = Guid.NewGuid(), DeskId = desks[2].Id, AmenityId = quiet.Id },
            new DeskAmenity { Id = Guid.NewGuid(), DeskId = desks[2].Id, AmenityId = monitor.Id },

            new DeskAmenity { Id = Guid.NewGuid(), DeskId = desks[3].Id, AmenityId = docking.Id },

            new DeskAmenity { Id = Guid.NewGuid(), DeskId = desks[4].Id, AmenityId = window.Id },
        };

        context.DeskAmenities.AddRange(deskAmenities);
        await context.SaveChangesAsync();

        // 3) Demo reservation (денес, за D-01)
        var demoReservation = new Reservation
        {
            Id = Guid.NewGuid(),
            DeskId = desks[0].Id,
            UserId = user!.Id,
            DateFrom = DateTime.Today,
            DateTo = DateTime.Today,
            Status = ReservationStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        context.Reservations.Add(demoReservation);
        await context.SaveChangesAsync();
    }
}
