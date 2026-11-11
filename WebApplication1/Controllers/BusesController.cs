using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using Route = WebApplication1.Models.Route;

namespace WebApplication1.Controllers;

[Authorize(Roles = "Admin")]
public class BusesController(TransportContext db) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        return View(await db.Vehicles.AsNoTracking().OrderBy(x => x.Make).ToListAsync());
    }

    public IActionResult Create()
    {
        return View("Form", new Vehicle());
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await db.Vehicles.FindAsync(id);
        if (item == null) return NotFound();
        return View("Form", item);
    }

    [HttpPost]
    public async Task<IActionResult> Save(int id, [Bind("Make,Model,LicensePlate")] Vehicle item)
    {
        item.VehicleID = id;
        if (ModelState.IsValid)
        {
            if (id == 0)
                db.Vehicles.Add(item);
            else
                db.Vehicles.Update(item);
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
        return View("Form", item);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await db.Vehicles.FindAsync(id);
        if (item == null) return NotFound();
        db.Vehicles.Remove(item);
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
}
