using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Controllers;

[ApiController]
[Route("api/routes")]
public class RoutesApiController(TransportContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetRoutes()
    {
        var routes = await db.Routes
            .OrderBy(r => r.RouteName)
            .Select(r => new
            {
                Name = r.RouteName,
                StartStop = r.StartStop!.LocationName,
                EndStop = r.EndStop!.LocationName
            })
            .ToListAsync();

        return Ok(routes);
    }
}