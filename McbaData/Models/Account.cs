using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace McbaData.Models
{
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int AccountNumber { get; set; }

        [Required]
        [RegularExpression("^[CS]$")]
        public char AccountType { get; set; }

        // Navigational property
        [Required]
        [ForeignKey(nameof(Customer))]
        public int CustomerID { get; set; }

        [InverseProperty(nameof(Transaction.Account))]
        public List<Transaction> Transactions { get; set; }

        // Navigational property
        public List<BillPay> BillPays { get; set; }
    }
}
