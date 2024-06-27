using System.Text.Json.Serialization;

namespace Ca.Backend.Test.Application.Models.Responses;
public class LineApiResponse
{
    [JsonPropertyName("productId")]
    public string ProductId { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("unit_price")]
    public int UnitPrice { get; set; }

    [JsonPropertyName("subtotal")]
    public int Subtotal { get; set; }
}

