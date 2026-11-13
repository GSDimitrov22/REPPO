using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[AllowAnonymous]
[IgnoreAntiforgeryToken]
public class ErrorsController : Controller
{
    [Route("errors/{code:int}")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public IActionResult Show(int code)
    {
        Response.StatusCode = code is >= 400 and <= 599 ? code : 404;
        return View(Response.StatusCode);
    }
}