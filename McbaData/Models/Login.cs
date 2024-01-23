using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McbaData.Models;

public class Login
{
    [Key]
    [Column(TypeName = "char(8)")]
    [RegularExpression(@"^\d{8}$")]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public required string LoginID { get; set; }

    // Navigational property
    [ForeignKey(nameof(Customer))]
    [Required]
    public int CustomerID { get; set; }
    public Customer Customer { get; set; }

    [Required]
    [Column(TypeName = "char(94)")]
    [StringLength(94)]
    public required string PasswordHash { get; set; }
}
