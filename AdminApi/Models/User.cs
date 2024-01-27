using System.ComponentModel.DataAnnotations;

namespace AdminApi.Models;

public class User
{
    [Required]
    public required String Username { get; set; }
    [Required]
    public required String Password { get; set; }
}