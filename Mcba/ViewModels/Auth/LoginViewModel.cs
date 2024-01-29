using System.ComponentModel.DataAnnotations;

namespace Mcba.ViewModels.Auth;

public class LoginViewModel
{
    [RegularExpression(@"^\d{8}$", ErrorMessage = "Login ID should be 8 digits")]
    [Required(ErrorMessage = "Login ID is required")]
    [Display(Name = "Login ID")]
    public required string LoginId { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}
