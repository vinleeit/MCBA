using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McbaData.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CustomerID { get; set; }

        [StringLength(50, ErrorMessage = "Name must be within 50 characters")]
        [Column(TypeName = "nvarchar(50)")]
        [Required(ErrorMessage = "Name is required")]
        public required string Name { get; set; }

        [Column(TypeName = "nvarchar(11)")]
        [RegularExpression(
            @"^\d{3} \d{3} \d{3}$",
            ErrorMessage = "TFN must be following this format (xxx xxx xxx)"
        )]
        public string? TFN { get; set; }

        [StringLength(50, ErrorMessage = "Address must be within 50 characters")]
        [Column(TypeName = "nvarchar(50)")]
        public string? Address { get; set; }

        [StringLength(40, ErrorMessage = "City must be within 50 characters")]
        [Column(TypeName = "nvarchar(40)")]
        public string? City { get; set; }

        [Column(TypeName = "nvarchar(3)")]
        [RegularExpression(
            @"^(NSW|VIC|QLD|WA|SA|TAS|ACT|NT)$",
            ErrorMessage = "Not a valid Australian state"
        )]
        public string? State { get; set; }

        [Column(TypeName = "nvarchar(4)")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Postcode must be 4 digits")]
        public string? Postcode { get; set; }

        [Column(TypeName = "nvarchar(12)")]
        [RegularExpression(
            @"^04\d{2} \d{3} \d{3}$",
            ErrorMessage = "Mobile must follow the following format (04xx xxx xxx)"
        )]
        public string? Mobile { get; set; }

        // Navigational property
        public List<Account> Accounts { get; set; }
        public Login Login { get; set; }
    }
}
