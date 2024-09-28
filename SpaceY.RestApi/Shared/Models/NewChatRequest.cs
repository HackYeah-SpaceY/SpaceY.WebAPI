using System.Text.Json.Serialization;

namespace SpaceY.RestApi.Shared.Models;

public class NewChatRequest
{
    [JsonPropertyName("id")]
    public Guid ChatId { get; set; }
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}
