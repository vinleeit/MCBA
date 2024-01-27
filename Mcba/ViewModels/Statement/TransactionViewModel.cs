using System.ComponentModel.DataAnnotations;

namespace Mcba.ViewModels.Statement;

public class TransactionViewModel
{
    [Key]
    [Display(Name = "Transaction ID")]
    public int TransactionID { get; set; }

    [Display(Name = "Transaction Type")]
    public required string TransactionType { get; set; }

    [Display(Name = "Account Number")]
    public int AccountNumber { get; set; }

    [Display(Name = "Destination Account Number")]
    public int? DestinationAccountNumber { get; set; }

    [Display(Name = "Amount (AU$)")]
    public decimal Amount { get; set; }

    [Display(Name = "Comment")]
    public string? Comment { get; set; }

    [Display(Name = "Transaction Time")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}")]
    public DateTime TransactionTimeLocal { get; set; }
}
