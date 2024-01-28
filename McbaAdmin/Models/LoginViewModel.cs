using System.ComponentModel.DataAnnotations;

namespace McbaAdmin.Models;
public class LoginViewModel
{
    [Required(ErrorMessage = "Username must not be empty")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Password must not be empty")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
}