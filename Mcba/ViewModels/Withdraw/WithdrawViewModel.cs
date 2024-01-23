using System.ComponentModel.DataAnnotations;
using McbaData.Models;

namespace Mcba.ViewModels.Withdraw;

public class WithdrawViewModel
{
    [Display(Name = "Account Number")]
    [Required(ErrorMessage = "Please select an account")]
    public int? AccountNumber { get; set; }

    [Required(ErrorMessage = "Please enter a valid amount")]
    [DataType(DataType.Currency, ErrorMessage = "Please enter a valid amount")]
    [Range(0.01, double.PositiveInfinity, ErrorMessage = "Amount must be at least $0.01")]
    public decimal Amount { get; set; }

    [MaxLength(30, ErrorMessage = "Comment must be within 30 characters")]
    public string? Comment { get; set; }

    // Output
    public List<Account>? Accounts { get; set; }
}
