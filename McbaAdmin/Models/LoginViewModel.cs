using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace McbaAdmin.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username must not be empty")]
        [JsonPropertyName("username")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password must not be empty")]
        [DataType(DataType.Password)]
        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }
}

