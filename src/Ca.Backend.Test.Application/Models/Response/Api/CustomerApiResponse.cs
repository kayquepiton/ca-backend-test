using System.Text.Json.Serialization;

namespace Ca.Backend.Test.Application.Models.Responses;
public class CustomerApiResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("address")]
    public string Address { get; set; }
}

