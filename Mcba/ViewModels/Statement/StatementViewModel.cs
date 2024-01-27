using System.ComponentModel.DataAnnotations;

namespace Mcba.ViewModels.Statement;

public class StatementViewModel
{
    [Display(Name = "Account Number")]
    public required int AccountNumber { get; set; }
    [Display(Name = "Total Balance")]
    public decimal TotalBalance { get; set; }
    public int Page { get; set; }
    public int TotalPage { get; set; }
    public required List<TransactionViewModel> Transactions { get; set; }
}