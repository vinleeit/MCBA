using System.ComponentModel.DataAnnotations;
using McbaData.Models;

namespace Mcba.ViewModels.Deposit;

public class DepositViewModel
{
    [Required(ErrorMessage = "Please choose an account")]
    public int? AccountNumber { get; set; }

    [Required(ErrorMessage = "Amount should not be empty")]
    [DataType(DataType.Currency, ErrorMessage = "Amount must be a valid decimal")]
    [Range(0.01, double.PositiveInfinity, ErrorMessage = "Amount must be at least $0.01")]
    public decimal Amount { get; set; } = (decimal)10.0;

    [MaxLength(30, ErrorMessage = "Comment must be maximum 30 characters")]
    public string? Comment { get; set; }

    // Output
    public List<Account>? Accounts { get; set; }
}
