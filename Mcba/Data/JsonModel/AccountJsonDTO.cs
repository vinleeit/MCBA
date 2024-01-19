namespace Mcba.Data.JsonModel
{
    public class AccountJsonDTO
    {
        public int AccountNumber { get; set; }
        public required string AccountType { get; set; }
        public int CustomerID { get; set; }
        public required List<TransactionJsonDTO> Transactions { get; set; }
    }
}
