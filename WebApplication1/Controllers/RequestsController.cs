using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

[Authorize(Roles = "Client,Admin")]
public class RequestsController(TransportContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var query = db.TravelRequests
            .Include(r => r.Schedule)
                .ThenInclude(s => s!.Route)
            .Include(r => r.User)
            .AsNoTracking()
            .AsQueryable();

        if (!User.IsInRole("Admin"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            query = query.Where(r => r.UserId == userId);
        }

        var requests = await query
            .OrderBy(r => r.Schedule!.DepartureTime)
            .ToListAsync();

        return View(requests);
    }

    [HttpPost]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> Create(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Forbid();

        // Проверката на курса и записването на заявката са в една транзакция.
        await using var transaction = await db.Database
            .BeginTransactionAsync(IsolationLevel.Serializable);

        var schedule = await db.Schedules.FindAsync(id);
        if (schedule == null)
            return NotFound();

        if (schedule.DepartureTime <= DateTime.Now)
        {
            TempData["Message"] = "Курсът вече е тръгнал.";
            return RedirectToAction("Index", "Schedules");
        }

        bool alreadyRequested = await db.TravelRequests
            .AnyAsync(r => r.ScheduleID == id && r.UserId == userId);

        if (alreadyRequested)
        {
            TempData["Message"] = "Вече имате заявка за този курс.";
            return RedirectToAction(nameof(Index));
        }

        db.TravelRequests.Add(new TravelRequest
        {
            ScheduleID = id,
            UserId = userId
        });

        try
        {
            await db.SaveChangesAsync();
            await transaction.CommitAsync();
            TempData["Message"] = "Заявката е записана.";
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
            TempData["Message"] = "Заявката не беше записана. Проверете списъка си и опитайте отново.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        var request = await db.TravelRequests.FindAsync(id);
        if (request == null)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!User.IsInRole("Admin") && request.UserId != userId)
            return Forbid();

        db.TravelRequests.Remove(request);

        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }
}