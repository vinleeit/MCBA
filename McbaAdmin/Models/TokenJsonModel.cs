using System.Text.Json.Serialization;

namespace McbaAdmin.Models;

public class TokenJsonModel
{
    [JsonPropertyName("token")]
    public required string Token { get; set; }
}