using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcba.Models;

public class Transaction
{
    [Key]
    public int TransactionID { get; set; }

    [Required]
    [RegularExpression("^[DWTSB]$")]
    public char TransactionType { get; set; }

    [ForeignKey(nameof(Account))]
    [Required]
    public int AccountNumber { get; set; }
    public Account Account { get; set; }

    [ForeignKey(nameof(DestinationAccount))]
    public int? DestinationAccountNumber { get; set; }
    public Account DestinationAccount { get; set; }

    [Required]
    // Money must be at least 0.01
    [Range(0.01, double.PositiveInfinity)]
    [Column(TypeName = "money")]
    public decimal Amount { get; set; }

    [MaxLength(30)]
    [Column(TypeName = "nvarchar(30)")]
    public string? Comment { get; set; }

    [Required]
    [Column(TypeName = "datetime2")]
    public DateTime TransactionTimeUtc { get; set; }
}
