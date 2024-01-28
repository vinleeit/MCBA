using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McbaData.Models;

public class Payee
{
    [Key]
    public int PayeeID { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [Column(TypeName = "nvarchar(50)")]
    [MaxLength(50, ErrorMessage = "Name must be within 50 characters")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Address is required")]
    [Column(TypeName = "nvarchar(50)")]
    [MaxLength(50, ErrorMessage = "Address must be within 50 characters")]
    public string Address { get; set; }

    [Required(ErrorMessage = "City is required")]
    [Column(TypeName = "nvarchar(40)")]
    [MaxLength(40, ErrorMessage = "City must be within 40 characters")]
    public string City { get; set; }

    [Required(ErrorMessage = "State is required")]
    [Column(TypeName = "nvarchar(3)")]
    [RegularExpression(
        @"^(NSW|VIC|QLD|WA|SA|TAS|ACT|NT)$",
        ErrorMessage = "Not an Australian state code"
    )]
    public string State { get; set; }

    [Required(ErrorMessage = "Postcode is required")]
    [Column(TypeName = "nvarchar(4)")]
    [RegularExpression(
        @"^[0-9]{4}$",
        ErrorMessage = "Postcode must be 4 digits"
    )]
    public string Postcode { get; set; }

    [Required(ErrorMessage = "Phone is required")]
    [Column(TypeName = "nvarchar(14)")]
    [RegularExpression(
        @"^\(0[0-9]\) [0-9]{4} [0-9]{4}$",
        ErrorMessage = "Phone must follow the format (0x xxxx xxxx)"
    )]
    public string Phone { get; set; }

    // Navigational property
    public List<BillPay> BillPays { get; set; }
}
