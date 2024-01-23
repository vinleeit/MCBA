using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mcba.Models;
using Microsoft.AspNetCore.Http.Timeouts;

public class Payee
{
    [Key]
    public int PayeeID { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(50)")]
    [MaxLength(50, ErrorMessage = "Max 50 characters")]
    public string Name { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(50)")]
    [MaxLength(50, ErrorMessage = "Max 50 characters")]
    public string Address { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(40)")]
    [MaxLength(40, ErrorMessage = "Max 40 characters")]
    public string City { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(3)")]
    [RegularExpression(@"^(NSW|VIC|QLD|WA|SA|TAS|ACT|NT)$", ErrorMessage = "Not an Australian state")]
    public string State { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(4)")]

    [RegularExpression(@"^[0-9]{4}$", ErrorMessage = "Must be 4 digits")]
    public string Postcode { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(14)")]
    [RegularExpression(@"^\(0[0-9]\) [0-9]{4} [0-9]{4}$", ErrorMessage = "Invalid format")]
    public string Phone { get; set; }

    // Navigational property
    public List<BillPay> BillPays { get; set; }
}