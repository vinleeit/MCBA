namespace Mcba.Data.JsonModel
{
    public class LoginJsonDTO
    {
        public required string LoginID { get; set; }
        public required string PasswordHash { get; set; }
    }
}
