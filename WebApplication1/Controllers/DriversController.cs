using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using Route = WebApplication1.Models.Route;

namespace WebApplication1.Controllers;

[Authorize(Roles = "Admin")]
public class DriversController(TransportContext db) : Controller
{
    
    public async Task<IActionResult> Index()
    {
        return View(await db.Drivers.AsNoTracking().OrderBy(x => x.FirstName).ToListAsync());
    }

    public IActionResult Create()
    {
        return View("Form", new Driver { HireDate = DateTime.Today });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await db.Drivers.FindAsync(id);
        if (item == null) return NotFound();
        return View("Form", item);
    }

    [HttpPost]
    public async Task<IActionResult> Save(int id, [Bind("FirstName,LastName,LicenseCategory,HireDate")] Driver item)
    {
        item.DriverID = id;
        if (ModelState.IsValid)
        {
            if (id == 0)
                db.Drivers.Add(item);
            else
                db.Drivers.Update(item);
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
        var item = await db.Drivers.FindAsync(id);
        if (item == null) return NotFound();
        db.Drivers.Remove(item);
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
