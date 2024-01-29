using System.ComponentModel.DataAnnotations;

namespace Mcba.ViewModels.Profile;

public class ProfileViewModel
{
    public int CustomerID { get; set; }

    [Required(ErrorMessage = "Please insert a name")]
    [StringLength(50, ErrorMessage = "Name must be at maximum 50 characters")]
    public string? Name { get; set; }

    [RegularExpression(
        @"^\d{3} \d{3} \d{3}$",
        ErrorMessage = "TFN must be formatted as 'xxx xxx xxx' where x is number"
    )]
    public string? TFN { get; set; }

    [StringLength(50, ErrorMessage = "Address must be at maximum 50 characters")]
    public string? Address { get; set; }

    [StringLength(40, ErrorMessage = "City must be at maximum 40 characters")]
    public string? City { get; set; }

    [RegularExpression(
        @"^(NSW|VIC|QLD|WA|SA|TAS|ACT|NT)$",
        ErrorMessage = "State must be a valid Australia state code"
    )]
    public string? State { get; set; }

    [RegularExpression(@"^\d{4}$", ErrorMessage = "Postcode must be an exact 4-digit number")]
    public string? Postcode { get; set; }

    [RegularExpression(
        @"^04\d{2} \d{3} \d{3}$",
        ErrorMessage = "Mobile must be formatted as '04xx xxx xxx' where x is number"
    )]
    public string? Mobile { get; set; }

    public string? ErrorMsg { get; set; }
}

