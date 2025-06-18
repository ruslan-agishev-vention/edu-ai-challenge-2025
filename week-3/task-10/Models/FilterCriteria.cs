using System.Text.Json.Serialization;

namespace ProductFilterApp.Models;

public class FilterCriteria
{
    [JsonPropertyName("category")]
    public string? Category { get; set; }

    [JsonPropertyName("max_price")]
    public decimal? MaxPrice { get; set; }

    [JsonPropertyName("min_price")]
    public decimal? MinPrice { get; set; }

    [JsonPropertyName("min_rating")]
    public decimal? MinRating { get; set; }

    [JsonPropertyName("in_stock_only")]
    public bool? InStockOnly { get; set; }

    [JsonPropertyName("search_term")]
    public string? SearchTerm { get; set; }
}
