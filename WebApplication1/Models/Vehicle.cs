using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Vehicle
{
    public int VehicleID { get; set; }

    [Display(Name = "Марка"), Required(ErrorMessage = "Полето е задължително."), StringLength(60, ErrorMessage = "Текстът е твърде дълъг.")]
    public string Make { get; set; } = string.Empty;

    [Display(Name = "Модел"), Required(ErrorMessage = "Полето е задължително."), StringLength(60, ErrorMessage = "Текстът е твърде дълъг.")]
    public string Model { get; set; } = string.Empty;

    [Display(Name = "Регистрационен номер"), Required(ErrorMessage = "Полето е задължително."), StringLength(20, ErrorMessage = "Текстът е твърде дълъг.")]
    public string LicensePlate { get; set; } = string.Empty;
}
