using System.ComponentModel.DataAnnotations;
using Mcba.Models;

namespace Mcba.ViewModels.BillPay;

public class BillPayViewModel
{
    [Key]
    public int BillPayID { get; set; }

    [Required]
    public int? AccountNumber { get; set; }

    [Required]
    public int PayeeID { get; set; }

    [Required]
    [DataType(DataType.Currency)]
    [Range(0.01, double.PositiveInfinity, ErrorMessage = "At least 0.01")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime ScheduleTimeLocal { get; set; }

    [Required]
    [RegularExpression("^[OM]$", ErrorMessage = "Only 'O' or 'M'")]
    public char? Period { get; set; }

    public string? PayeeName { get; set; }
    public List<Account>? Accounts { get; set; }
}