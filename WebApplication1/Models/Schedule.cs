using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Schedule
{
    public int ScheduleID { get; set; }

    [Display(Name = "Маршрут"), Range(1, int.MaxValue, ErrorMessage = "Изберете стойност от списъка.")]
    public int RouteID { get; set; }

    public Route? Route { get; set; }

    [Display(Name = "Автобус"), Range(1, int.MaxValue, ErrorMessage = "Изберете стойност от списъка.")]
    public int VehicleID { get; set; }

    public Vehicle? Vehicle { get; set; }

    [Display(Name = "Шофьор"), Range(1, int.MaxValue, ErrorMessage = "Изберете стойност от списъка.")]
    public int DriverID { get; set; }

    public Driver? Driver { get; set; }

    [Display(Name = "Заминаване"), DataType(DataType.DateTime)]
    public DateTime DepartureTime { get; set; }

    [Display(Name = "Пристигане"), DataType(DataType.DateTime)]
    public DateTime ArrivalTime { get; set; }
}
