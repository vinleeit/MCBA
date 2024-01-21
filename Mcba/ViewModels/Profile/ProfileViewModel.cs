using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcba.ViewModels.Profile;

public class ProfileViewModel
{
    public int CustomerID { get; set; }

    [StringLength(50)]
    [Column(TypeName = "nvarchar(50)")]
    [Required]
    public required string Name { get; set; }

    [Column(TypeName = "nvarchar(11)")]
    [RegularExpression(@"^\d{3} \d{3} \d{3}$")]
    public string? TFN { get; set; }

    [StringLength(50)]
    [Column(TypeName = "nvarchar(50)")]
    public string? Address { get; set; }

    [StringLength(40)]
    [Column(TypeName = "nvarchar(40)")]
    public string? City { get; set; }

    [Column(TypeName = "nvarchar(3)")]
    [RegularExpression(@"^(NSW|VIC|QLD|WA|SA|TAS|ACT|NT)$")]
    public string? State { get; set; }

    [Column(TypeName = "nvarchar(4)")]
    [RegularExpression(@"^\d{4}$")]
    public string? Postcode { get; set; }

    [Column(TypeName = "nvarchar(12)")]
    [RegularExpression(@"^04\d{2} \d{3} \d{3}$")]
    public string? Mobile { get; set; }
}