using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McbaData.Models;

public class BillPay
{
    [Key]
    public int BillPayID { get; set; }

    // Navigational property
    [Required(ErrorMessage = "Account Number is required")]
    [ForeignKey(nameof(Account))]
    public int AccountNumber { get; set; }
    public Account Account { get; set; }

    // Navigational property
    [Required(ErrorMessage = "Payee is required")]
    [ForeignKey(nameof(Payee))]
    public int PayeeID { get; set; }
    public Payee Payee { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Column(TypeName = "money")]
    [Range(0.01, double.PositiveInfinity, ErrorMessage = "At least 0.01")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Schedule time is required")]
    [Column(TypeName = "datetime2")]
    public DateTime ScheduleTimeUtc { get; set; }

    [Required(ErrorMessage = "Period is required")]
    [RegularExpression("^[OM]$", ErrorMessage = "Only 'O' or 'M'")]
    public char Period { get; set; }
}
