using Microsoft.EntityFrameworkCore;
using VehicleInventory.Application.Abstractions;
using VehicleInventory.Domain.Entities;
using VehicleInventory.Domain.Interfaces;
using VehicleInventory.Domain.ValueObjects;
using VehicleInventory.Infrastructure.Persistence;

namespace VehicleInventory.API.Extensions;

/// <summary>
/// Seeds demo service advisors on first startup (only when table is empty).
/// Password for all demo advisors: Demo1234!
/// </summary>
public static class DatabaseSeedExtension
{
    private static readonly (string First, string Last, string Email, string Dept)[] DemoAdvisors =
    [
        ("Wendy",  "Klorp",   "w.klorp@aio-demo.com",   "Satış"),
        ("Rex",    "Dunbar",  "r.dunbar@aio-demo.com",  "Teknik Servis"),
        ("Jill",   "Sprock",  "j.sprock@aio-demo.com",  "VIP Müşteri Hizmetleri"),
    ];

    public static async Task SeedServiceAdvisorsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context       = scope.ServiceProvider.GetRequiredService<VehicleInventoryDbContext>();
        var repository    = scope.ServiceProvider.GetRequiredService<IServiceAdvisorRepository>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        // Only seed if the table is completely empty
        if (await context.ServiceAdvisors.AnyAsync())
            return;

        app.Logger.LogInformation("Seeding demo service advisors...");

        foreach (var (first, last, email, dept) in DemoAdvisors)
        {
            var emailResult = Email.Create(email);
            if (emailResult.IsFailure)
            {
                app.Logger.LogWarning("Invalid seed email {Email}: {Error}", email, emailResult.Error);
                continue;
            }

            var hash   = passwordHasher.Hash("Demo1234!");
            var result = ServiceAdvisor.Create(first, last, emailResult.Value!, hash, dept);

            if (result.IsFailure)
            {
                app.Logger.LogWarning("Could not create advisor {Email}: {Error}", email, result.Error);
                continue;
            }

            await repository.AddAsync(result.Value!);
            app.Logger.LogInformation("  Created advisor: {DisplayName} <{Email}>",
                result.Value!.GetDisplayName(), email);
        }
    }
}
