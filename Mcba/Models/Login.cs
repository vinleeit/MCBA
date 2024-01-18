using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcba.Models;

public class Login
{
    [Column(TypeName = "char(8)")]
    [Required]
    [Length(8, 8)]
    public required string LoginID { get; set; }

    // Navigational property
    [ForeignKey(nameof(Customer))]
    [Required]
    public int CustomerID { get; set; }
    public Customer Customer { get; set; }

    [Column(TypeName = "char(94)")]
    [Required]
    [Length(94, 94)]
    public required string PasswordHash { get; set; }
}
