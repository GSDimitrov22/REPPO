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
        public DbSet<TravelRequest> TravelRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder model)
        {
            base.OnModelCreating(model);
            model.Entity<Route>().HasOne(r => r.StartStop).WithMany()
                .HasForeignKey(r => r.StartStopID).OnDelete(DeleteBehavior.Restrict);
            model.Entity<Route>().HasOne(r => r.EndStop).WithMany()
                .HasForeignKey(r => r.EndStopID).OnDelete(DeleteBehavior.Restrict);
            model.Entity<Schedule>().HasOne(s => s.Route).WithMany()
                .HasForeignKey(s => s.RouteID).OnDelete(DeleteBehavior.Restrict);
            model.Entity<Schedule>().HasOne(s => s.Vehicle).WithMany()
                .HasForeignKey(s => s.VehicleID).OnDelete(DeleteBehavior.Restrict);
            model.Entity<Schedule>().HasOne(s => s.Driver).WithMany()
                .HasForeignKey(s => s.DriverID).OnDelete(DeleteBehavior.Restrict);
            model.Entity<TravelRequest>().HasOne(r => r.Schedule).WithMany()
                .HasForeignKey(r => r.ScheduleID).OnDelete(DeleteBehavior.Restrict);
            model.Entity<TravelRequest>().HasIndex(r => new { r.UserId, r.ScheduleID }).IsUnique();
            model.Entity<Stop>().HasIndex(s => s.LocationName).IsUnique();
            model.Entity<Route>().HasIndex(r => r.RouteName).IsUnique();
            model.Entity<Vehicle>().HasIndex(v => v.LicensePlate).IsUnique();
            model.Entity<Route>().ToTable(t => t.HasCheckConstraint("CK_Route_Stops", "StartStopID <> EndStopID"));
            model.Entity<Schedule>().ToTable(t => t.HasCheckConstraint("CK_Schedule_Time", "ArrivalTime > DepartureTime"));
        }

        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Stop> Stops { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
    }
}