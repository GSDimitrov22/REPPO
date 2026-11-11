using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Stop
{
    public int StopID { get; set; }

    [Display(Name = "Име на спирката"), Required(ErrorMessage = "Полето е задължително."), StringLength(60, ErrorMessage = "Текстът е твърде дълъг.")]
    public string LocationName { get; set; } = string.Empty;
}
