using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcba.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CustomerID { get; set; }

        [StringLength(50)]
        [Column("nvarchar(50)")]
        [Required]
        public required string Name { get; set; }

        [Column("nvarchar(11)")]
        [RegularExpression(@"^\d{3} \d{3} \d{3}$")]
        public string? TFN { get; set; }

        [StringLength(50)]
        [Column("nvarchar(50)")]
        public string? Address { get; set; }

        [StringLength(40)]
        [Column("nvarchar(40)")]
        public string? City { get; set; }

        [Column("nvarchar(3)")]
        [RegularExpression(@"^(NSW|VIC|QLD|WA|SA|TAS|ACT|NT)$")]
        public string? State { get; set; }

        [Column("nvarchar(4)")]
        [RegularExpression(@"^\d{4}$")]
        public string? Postcode { get; set; }

        [Column("nvarchar(12)")]
        [RegularExpression(@"^04\d{2} \d{3} \d{3}$")]
        public string? Mobile { get; set; }

        // Navigational property
        public List<Account> Accounts { get; set; }
        public Login Login { get; set; }
    }
}
