using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McbaData.Models;

public class Transaction
{
    [Key]
    public int TransactionID { get; set; }

    [Required]
    [RegularExpression(
        "^[DWTSB]$",
        ErrorMessage = "Transaction type should be in [D, W, T, S, B]"
    )]
    public char TransactionType { get; set; }

    // Navigational property
    [ForeignKey(nameof(Account))]
    [Required(ErrorMessage = "Account number is required")]
    public int AccountNumber { get; set; }
    public Account Account { get; set; }

    // Navigational property
    [ForeignKey(nameof(DestinationAccount))]
    public int? DestinationAccountNumber { get; set; }
    public Account DestinationAccount { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Column(TypeName = "money")]
    [Range(0.01, double.PositiveInfinity, ErrorMessage = "Amount must be at least $0.01")]
    public decimal Amount { get; set; }

    [Column(TypeName = "nvarchar(30)")]
    [MaxLength(30, ErrorMessage = "Comment must be within 30 characters")]
    public string? Comment { get; set; }

    [Required(ErrorMessage = "Transaction time UTC is required")]
    [Column(TypeName = "datetime2")]
    public DateTime TransactionTimeUtc { get; set; }
}
