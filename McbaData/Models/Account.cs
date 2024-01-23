using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McbaData.Models
{
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AccountNumber { get; set; }

        [Required(ErrorMessage = "Account type is required")]
        [RegularExpression(
            "^[CS]$",
            ErrorMessage = "Account type must be Checking (C) or Saving (S)"
        )]
        public char AccountType { get; set; }

        // Navigational property
        [Required(ErrorMessage = "Customer ID is required")]
        [ForeignKey(nameof(Customer))]
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }

        [InverseProperty(nameof(Transaction.Account))]
        public List<Transaction> Transactions { get; set; }

        // Navigational property
        public List<BillPay> BillPays { get; set; }
    }
}
