namespace Mcba.Data.JsonModel
{
    public class TransactionJsonDTO
    {
        public double Amount { get; set; }
        public string? Comment { get; set; }
        public required DateTime TransactionTimeUtc { get; set; }
    }
}
