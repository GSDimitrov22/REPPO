using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Driver
{
    public int DriverID { get; set; }

    [Display(Name = "Име"), Required(ErrorMessage = "Полето е задължително."), StringLength(60, ErrorMessage = "Текстът е твърде дълъг.")]
    public string FirstName { get; set; } = string.Empty;

    [Display(Name = "Фамилия"), Required(ErrorMessage = "Полето е задължително."), StringLength(60, ErrorMessage = "Текстът е твърде дълъг.")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Категория"), Required(ErrorMessage = "Полето е задължително."), StringLength(60, ErrorMessage = "Текстът е твърде дълъг."), RegularExpression("D1|D|D1E|DE", ErrorMessage = "Допустими категории: D1, D, D1E, DE.")]
    public string LicenseCategory { get; set; } = string.Empty;

    [Display(Name = "Дата на назначаване"), DataType(DataType.Date)]
    public DateTime HireDate { get; set; }
}
