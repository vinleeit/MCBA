namespace Mcba.Data.JsonModel
{
    public class CustomerJsonDTO
    {
        public int CustomerID { get; set; }
        public required string Name { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PostCode { get; set; }
        public required List<AccountJsonDTO> Accounts { get; set; }
        public required LoginJsonDTO Login { get; set; }
    }
}
