using System.Text.Json.Serialization;

namespace TchiBot.Api.Models;

public class TariffInfoDto
{
    [JsonPropertyName("tariffInfoSummaryBean")]
    public TariffInfoBeanDto TariffInfoBean { get; set; }
}

public class TariffInfoBeanDto
{
    [JsonPropertyName("bookingInlifeDate")]
    public string ExtendsOn { get; set; }
}
