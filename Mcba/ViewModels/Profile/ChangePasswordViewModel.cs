using System.ComponentModel.DataAnnotations;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Please insert a new password")]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
}