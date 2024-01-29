using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McbaData.Models;

public class Login
{
    [Key]
    [Required(ErrorMessage = "Login ID is required")]
    [Column(TypeName = "char(8)")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "Login ID must be 8 digits")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required string LoginID { get; set; }

    // Navigational property
    [ForeignKey(nameof(Customer))]
    [Required(ErrorMessage = "Customer ID is required")]
    [Range(1000, 9999, ErrorMessage = "Account number must be 4 digits")]
    public int CustomerID { get; set; }
    public Customer Customer { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [Column(TypeName = "char(94)")]
    [StringLength(94, ErrorMessage = "Wrong password length")]
    public required string PasswordHash { get; set; }

    // To indicate if a user account is locked
    [Required]
    [DefaultValue(false)]
    public bool Locked { get; set; }
}
