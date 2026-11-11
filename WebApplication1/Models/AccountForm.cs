using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class AccountForm
{
    [Required, EmailAddress, StringLength(256), Display(Name = "Имейл")]
    public string Email { get; set; } = string.Empty;

    [Required, DataType(DataType.Password), Display(Name = "Парола")]
    public string Password { get; set; } = string.Empty;
}
