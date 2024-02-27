using System.Text.Json.Serialization;

namespace HK_ArbitrageBot.Models;

internal class Security
{
    [JsonPropertyName("symbol")]
    public string? Symbol { get; set; }
    
    [JsonPropertyName("ask")]
    public decimal? Ask { get; set; }

    [JsonPropertyName("prev_close_price")]
    public decimal? PrevClosePrice { get; set; }

    [JsonPropertyName("last_price")]
    public decimal? LastPrice { get; set; }

    [JsonPropertyName("exchange")]
    public string Exchange { get; set; }
}
