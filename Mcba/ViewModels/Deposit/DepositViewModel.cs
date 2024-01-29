using System.ComponentModel.DataAnnotations;
using McbaData.Models;

namespace Mcba.ViewModels.Deposit;

public class DepositViewModel
{
    [Required(ErrorMessage = "Please select an account")]
    [Display(Name = "Account Number")]
    public int? AccountNumber { get; set; }

    [Required(ErrorMessage = "Please insert an amount")]
    [Display(Name = "Amount (AU$)")]
    [DataType(DataType.Currency, ErrorMessage = "Amount must be a valid number")]
    [Range(0.01, double.PositiveInfinity, ErrorMessage = "Amount must be at minimum AU$0.01")]
    public decimal? Amount { get; set; } = (decimal)10.0;

    [MaxLength(30, ErrorMessage = "Comment must be at maximum 30 characters")]
    public string? Comment { get; set; }

    // Output
    public List<Account>? Accounts { get; set; }
}
