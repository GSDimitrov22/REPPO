using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Route
{
    public int RouteID { get; set; }

    [Display(Name = "Линия"), Required(ErrorMessage = "Полето е задължително."), StringLength(60, ErrorMessage = "Текстът е твърде дълъг.")]
    public string RouteName { get; set; } = string.Empty;

    [Display(Name = "Описание"), Required(ErrorMessage = "Полето е задължително."), StringLength(200, ErrorMessage = "Текстът е твърде дълъг.")]
    public string Description { get; set; } = string.Empty;

    [Display(Name = "Начална спирка"), Range(1, int.MaxValue, ErrorMessage = "Изберете стойност от списъка.")]
    public int StartStopID { get; set; }

    public Stop? StartStop { get; set; }

    [Display(Name = "Крайна спирка"), Range(1, int.MaxValue, ErrorMessage = "Изберете стойност от списъка.")]
    public int EndStopID { get; set; }

    public Stop? EndStop { get; set; }
}
