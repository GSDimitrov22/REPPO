using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using Route = WebApplication1.Models.Route;
namespace WebApplication1.Data
{
    public class TransportContext : IdentityDbContext
    {
        public TransportContext(DbContextOptions<TransportContext> options)
            : base(options)
        {
        }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Stop> Stops { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
    }
}