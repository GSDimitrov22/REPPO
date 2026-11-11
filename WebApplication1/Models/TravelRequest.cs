using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models;

public class TravelRequest
{
    public int TravelRequestID { get; set; }
    public int ScheduleID { get; set; }
    public Schedule? Schedule { get; set; }
    public string UserId { get; set; } = string.Empty;
    public IdentityUser? User { get; set; }
}
