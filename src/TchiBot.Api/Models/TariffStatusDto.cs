using System.Text.Json.Serialization;

namespace TchiBot.Api.Models;

public class TariffStatusDto
{
    [JsonPropertyName("tariffStatusBeans")]
    public List<TariffStatusBeanDto> TariffStatusBeans { get; set; }
}

public class TariffStatusBeanDto
{
    [JsonPropertyName("currentValue")]
    public int CurrentValue { get; set; }

    [JsonPropertyName("freePercent")]
    public string FreePercent { get; set; }

    [JsonPropertyName("isThrottled")]
    public bool IsThrottled { get; set; }

    [JsonPropertyName("maxValue")]
    public int MaxValue { get; set; }

    [JsonPropertyName("usedPercent")]
    public string UsedPercent { get; set; }
}
