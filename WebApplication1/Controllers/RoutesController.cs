using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using Route = WebApplication1.Models.Route;

namespace WebApplication1.Controllers;

[Authorize(Roles = "Admin")]
public class RoutesController(TransportContext db) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        return View(await db.Routes.AsNoTracking().Include(x => x.StartStop).Include(x => x.EndStop).OrderBy(x => x.RouteName).ToListAsync());
    }

    public async Task<IActionResult> Create()
    {
        await LoadChoices();
        return View("Form", new Route());
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await db.Routes.FindAsync(id);
        if (item == null) return NotFound();
        await LoadChoices();
        return View("Form", item);
    }

    [HttpPost]
    public async Task<IActionResult> Save(int id, [Bind("RouteName,Description,StartStopID,EndStopID")] Route item)
    {
        item.RouteID = id;
        if (item.StartStopID == item.EndStopID)
            ModelState.AddModelError("EndStopID", "Изберете две различни спирки.");
        if (ModelState.IsValid)
        {
            if (id == 0)
                db.Routes.Add(item);
            else
                db.Routes.Update(item);
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
        var item = await db.Routes.FindAsync(id);
        if (item == null) return NotFound();
        db.Routes.Remove(item);
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
        ViewBag.StartStopID = new SelectList(await db.Stops.OrderBy(x => x.LocationName).ToListAsync(), "StopID", "LocationName");
        ViewBag.EndStopID = new SelectList(await db.Stops.OrderBy(x => x.LocationName).ToListAsync(), "StopID", "LocationName");
    }
}
