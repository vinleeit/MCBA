using System.ComponentModel.DataAnnotations;

namespace McbaData.Dtos;

public class CustomerDto
{
    public int CustomerId { get; set; }

    [StringLength(50, ErrorMessage = "Name must be within 50 characters")]
    [Required(ErrorMessage = "Name is required")]
    public required string Name { get; set; }

    [RegularExpression(
        @"^\d{3} \d{3} \d{3}$",
        ErrorMessage = "TFN must be following this format (xxx xxx xxx)"
    )]
    public string? TFN { get; set; }

    [StringLength(50, ErrorMessage = "Address must be within 50 characters")]
    public string? Address { get; set; }

    [StringLength(40, ErrorMessage = "City must be within 50 characters")]
    public string? City { get; set; }

    [RegularExpression(
        @"^(NSW|VIC|QLD|WA|SA|TAS|ACT|NT)$",
        ErrorMessage = "Not a valid Australian state"
    )]
    public string? State { get; set; }

    [RegularExpression(@"^\d{4}$", ErrorMessage = "Postcode must be 4 digits")]
    public string? Postcode { get; set; }

    [RegularExpression(
        @"^04\d{2} \d{3} \d{3}$",
        ErrorMessage = "Mobile must follow the following format (04xx xxx xxx)"
    )]
    public string? Mobile { get; set; }
}
