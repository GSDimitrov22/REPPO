using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using Route = WebApplication1.Models.Route;

namespace WebApplication1.Controllers;

[Authorize(Roles = "Admin")]
public class SchedulesController(TransportContext db) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index(string? search, int? routeId, string sort = "time", int page = 1)
    {
        var query = db.Schedules.AsNoTracking().Include(x => x.Route).Include(x => x.Vehicle)
            .Include(x => x.Driver).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            query = query.Where(x => x.Route!.RouteName.Contains(search)
                || x.Route.Description.Contains(search) || x.Vehicle!.LicensePlate.Contains(search));
        }
        if (routeId.HasValue)
            query = query.Where(x => x.RouteID == routeId);

        const int pageSize = 5;
        int pages = Math.Max(1, (await query.CountAsync() + pageSize - 1) / pageSize);
        page = Math.Clamp(page, 1, pages);
        query = sort switch
        {
            "line" => query.OrderBy(x => x.Route!.RouteName).ThenBy(x => x.ScheduleID),
            "latest" => query.OrderByDescending(x => x.DepartureTime).ThenBy(x => x.ScheduleID),
            _ => query.OrderBy(x => x.DepartureTime).ThenBy(x => x.ScheduleID)
        };
        ViewBag.Search = search;
        ViewBag.RouteId = routeId;
        ViewBag.Sort = sort;
        ViewBag.Page = page;
        ViewBag.Pages = pages;
        ViewBag.Lines = await db.Routes.OrderBy(x => x.RouteName).ToListAsync();
        return View(await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        await LoadChoices();
        return View("Form", new Schedule { DepartureTime = DateTime.Today.AddDays(1).AddHours(8), ArrivalTime = DateTime.Today.AddDays(1).AddHours(9) });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await db.Schedules.FindAsync(id);
        if (item == null) return NotFound();
        await LoadChoices();
        return View("Form", item);
    }

    [HttpPost]
    public async Task<IActionResult> Save(int id, [Bind("RouteID,VehicleID,DriverID,DepartureTime,ArrivalTime")] Schedule item)
    {
        item.ScheduleID = id;
        if (item.ArrivalTime <= item.DepartureTime)
            ModelState.AddModelError("ArrivalTime", "Пристигането трябва да е след заминаването.");
        if (ModelState.IsValid)
        {
            if (id == 0)
                db.Schedules.Add(item);
            else
                db.Schedules.Update(item);
            try
            {
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Проверете за дублирана стойност или невалидна връзка.");
            }
        }
        await LoadChoices();
        return View("Form", item);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await db.Schedules.FindAsync(id);
        if (item == null) return NotFound();
        db.Schedules.Remove(item);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            TempData["Message"] = "Записът се използва и не може да бъде изтрит.";
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadChoices()
    {
        ViewBag.RouteID = new SelectList(await db.Routes.OrderBy(x => x.RouteName).ToListAsync(), "RouteID", "RouteName");
        ViewBag.VehicleID = new SelectList(await db.Vehicles.OrderBy(x => x.LicensePlate).ToListAsync(), "VehicleID", "LicensePlate");
        ViewBag.DriverID = new SelectList(await db.Drivers.OrderBy(x => x.LastName).ToListAsync(), "DriverID", "LastName");
    }
}
