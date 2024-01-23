using System.ComponentModel.DataAnnotations;
using McbaData.Models;

namespace Mcba.ViewModels.Transfer;

public class TransferViewModel
{
    [Display(Name = "Account Number")]
    [Required(ErrorMessage = "Please select an account")]
    public int? AccountNumber { get; set; }

    [Display(Name = "Destintion Account Number")]
    [Required(ErrorMessage = "Please input destination account number")]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "Account number must be 4 digit number")]
    public string? DestinationAccountNumber { get; set; }

    [DataType(DataType.Currency, ErrorMessage = "Please input a valid amount")]
    [Required(ErrorMessage = "Please input an amount")]
    [Range(0.01, double.PositiveInfinity, ErrorMessage = "Transfer must be at least $0.01")]
    public decimal Amount { get; set; }

    [MaxLength(30, ErrorMessage = "Comment must be in 30 characters")]
    public string? Comment { get; set; }

    // Output
    public List<Account>? Accounts { get; set; }
}

