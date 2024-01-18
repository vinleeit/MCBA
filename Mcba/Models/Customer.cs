using System.ComponentModel.DataAnnotations;

namespace Mcba.Models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }

        [MaxLength(50)]
        [Required]
        public required string Name { get; set; }

        [Length(11, 11)]
        [RegularExpression("[0-9]{3} [0-9]{3} [0-9]{3}")]
        public string? TFN { get; set; }

        [MaxLength(50)]
        public string? Address { get; set; }

        [MaxLength(40)]
        public string? City { get; set; }

        [Length(2, 3)]
        public string? State { get; set; }

        [Length(4, 4)]
        public string? Postcode { get; set; }

        [Length(12, 12)]
        [RegularExpression("04[0-9]{2} [0-9]{3} [0-9]{3}")]
        public string? Mobile { get; set; }
    }
}
