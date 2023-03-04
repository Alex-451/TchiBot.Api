using System.Text.Json.Serialization;

namespace TchiBot.Api.Models;

public class SecurityTokenDto
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("securityToken")]
    public string SecurityToken { get; set; } = string.Empty;
}
