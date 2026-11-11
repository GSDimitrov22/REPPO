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
        var query = db.TravelRequests.Include(r => r.Schedule).ThenInclude(s => s!.Route)
            .Include(r => r.User).AsNoTracking().AsQueryable();
        if (!User.IsInRole("Admin"))
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            query = query.Where(r => r.UserId == userId);
        }
        return View(await query.OrderBy(r => r.Schedule!.DepartureTime).ToListAsync());
    }

    [HttpPost, Authorize(Roles = "Client")]
    public async Task<IActionResult> Create(int id)
    {
        var schedule = await db.Schedules.FindAsync(id);
        if (schedule == null) return NotFound();
        if (schedule.DepartureTime <= DateTime.Now)
        {
            TempData["Message"] = "Курсът вече е тръгнал.";
            return RedirectToAction("Index", "Schedules");
        }
        db.TravelRequests.Add(new TravelRequest
        {
            ScheduleID = id, UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!
        });
        try
        {
            await db.SaveChangesAsync();
            TempData["Message"] = "Заявката е записана.";
        }
        catch (DbUpdateException)
        {
            TempData["Message"] = "Вече имате заявка или курсът вече не е наличен.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        var request = await db.TravelRequests.FindAsync(id);
        if (request == null) return NotFound();
        if (!User.IsInRole("Admin") && request.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            return Forbid();
        db.TravelRequests.Remove(request);
        try { await db.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException) { return NotFound(); }
        return RedirectToAction(nameof(Index));
    }
}
