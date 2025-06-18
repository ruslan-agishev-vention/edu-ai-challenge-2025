using System.Text.Json.Serialization;

namespace ProductFilterApp.Models;

public class Product
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("rating")]
    public decimal Rating { get; set; }

    [JsonPropertyName("in_stock")]
    public bool InStock { get; set; }

    public override string ToString()
    {
        return $"{Name} - ${Price:F2}, Rating: {Rating}, {(InStock ? "In Stock" : "Out of Stock")}";
    }
}
