namespace WebApplication1.Models
{
    public class Schedule
    {
        public int ScheduleID { get; set; }
        public int RouteID { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
    }
}