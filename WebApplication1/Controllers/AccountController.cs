using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

public class AccountController(UserManager<IdentityUser> users,
    SignInManager<IdentityUser> signIn, TransportContext db) : Controller
{
    public IActionResult Login() => View("Form", new AccountForm());
    public IActionResult Register() => View("Form", new AccountForm());
    public IActionResult Denied() => View();

    [HttpPost]
    public async Task<IActionResult> Register(AccountForm form)
    {
        if (!ModelState.IsValid) return View("Form", form);
        using var transaction = await db.Database.BeginTransactionAsync();
        var user = new IdentityUser { UserName = form.Email.Trim(), Email = form.Email.Trim() };
        var result = await users.CreateAsync(user, form.Password);
        if (result.Succeeded)
            result = await users.AddToRoleAsync(user, "Client");
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
            return View("Form", form);
        }
        await transaction.CommitAsync();
        await signIn.SignInAsync(user, false);
        return RedirectToAction("Index", "Schedules");
    }

    [HttpPost]
    public async Task<IActionResult> Login(AccountForm form, string? returnUrl)
    {
        if (!ModelState.IsValid) return View("Form", form);
        var result = await signIn.PasswordSignInAsync(form.Email.Trim(), form.Password, false, true);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Неуспешен вход. Проверете данните или опитайте по-късно.");
            return View("Form", form);
        }
        if (Url.IsLocalUrl(returnUrl)) return LocalRedirect(returnUrl!);
        return RedirectToAction("Index", "Schedules");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await signIn.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
