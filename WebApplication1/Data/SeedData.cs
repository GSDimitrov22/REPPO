using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using Route = WebApplication1.Models.Route;

namespace WebApplication1.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider services, bool demo)
    {
        var db = services.GetRequiredService<TransportContext>();
        await db.Database.EnsureCreatedAsync();
        var roles = services.GetRequiredService<RoleManager<IdentityRole>>();
        var users = services.GetRequiredService<UserManager<IdentityUser>>();
        using var transaction = await db.Database.BeginTransactionAsync();
        // Ролите и началните записи се записват заедно при първо стартиране.
        if (!await roles.RoleExistsAsync("Client"))
        {
            Check(await roles.CreateAsync(new IdentityRole("Client")));
            Check(await roles.CreateAsync(new IdentityRole("Admin")));
            var station = new Stop { LocationName = "ЖП гара" };
            var centre = new Stop { LocationName = "Център" };
            var terminal = new Stop { LocationName = "Терминал Меден рудник" };
            var lines = new[]
            {
                new Route { RouteName = "Б1", Description = "Меден рудник – Център", StartStop = terminal, EndStop = centre },
                new Route { RouteName = "Б2", Description = "Център – ЖП гара", StartStop = centre, EndStop = station }
            };
            var buses = new[]
            {
                new Vehicle { Make = "MAN", Model = "Lion’s City", LicensePlate = "А 1201 АА" },
                new Vehicle { Make = "Solaris", Model = "Urbino", LicensePlate = "А 1202 АА" }
            };
            var drivers = new[]
            {
                new Driver { FirstName = "Иван", LastName = "Петров", LicenseCategory = "D", HireDate = new DateTime(2022, 3, 1) },
                new Driver { FirstName = "Мария", LastName = "Колева", LicenseCategory = "D", HireDate = new DateTime(2023, 5, 8) }
            };
            for (int i = 0; i < 12; i++)
            {
                var departure = DateTime.Today.AddDays(1 + i / 6).AddHours(8 + i % 6);
                db.Schedules.Add(new Schedule
                {
                    Route = lines[i % 2], Vehicle = buses[i % 2], Driver = drivers[i % 2],
                    DepartureTime = departure, ArrivalTime = departure.AddMinutes(30)
                });
            }
            await db.SaveChangesAsync();
        }
        if (demo)
        {
            await DemoUser(users, "admin@reppo.test", "Admin", "Admin123!");
            await DemoUser(users, "client@reppo.test", "Client", "Client123!");
        }
        await transaction.CommitAsync();
    }

    private static async Task DemoUser(UserManager<IdentityUser> users, string email, string role, string password)
    {
        if (await users.FindByEmailAsync(email) != null) return;
        var user = new IdentityUser { Email = email, UserName = email };
        Check(await users.CreateAsync(user, password));
        Check(await users.AddToRoleAsync(user, role));
    }

    private static void Check(IdentityResult result)
    {
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}
