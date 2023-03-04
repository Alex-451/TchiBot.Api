using System.Text.Json.Serialization;

namespace TchiBot.Api.Models;

public class PublicKeyDto
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("publicKey")]
    public string PublicKey { get; set; }
}
