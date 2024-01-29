using System.ComponentModel.DataAnnotations;
using McbaData.Models;

namespace Mcba.ViewModels.BillPay;

public class BillPayViewModel
{
    [Key]
    public int BillPayID { get; set; }

    [Required(ErrorMessage = "Please select an account")]
    [Display(Name = "Account Number")]
    public int? AccountNumber { get; set; }

    [Required(ErrorMessage = "Please insert a payee ID")]
    [Display(Name = "Payee ID")]
    [Range(0, double.PositiveInfinity, ErrorMessage = "Payee ID must be number")]
    public int? PayeeID { get; set; }

    [Required(ErrorMessage = "Please insert an amount")]
    [Display(Name = "Amount (AU$)")]
    [DataType(DataType.Currency, ErrorMessage = "Amount must be a valid number")]
    [Range(0.01, double.PositiveInfinity, ErrorMessage = "Amount must be at minimum AU$0.01")]
    public decimal? Amount { get; set; }

    [Required(ErrorMessage = "Please insert a payment schedule")]
    [Display(Name = "Payment Schedule")]
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh:mm tt}")]
    public DateTime ScheduleTimeLocal { get; set; }

    [Required(ErrorMessage = "Please select a period")]
    [RegularExpression("^[OM]$", ErrorMessage = "Period must be 'One-time' or 'Monthly'")]
    public char? Period { get; set; }

    public string? PayeeName { get; set; }
    public List<Account>? Accounts { get; set; }
}