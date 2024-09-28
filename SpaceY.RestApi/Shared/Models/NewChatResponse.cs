using System.Text.Json.Serialization;

namespace SpaceY.RestApi.Shared.Models;

public class NewChatResponse
{

    [JsonPropertyName("status")]
    public string? status { get; set; }
    [JsonPropertyName("response")]
    public string? Response { get; set; }
}
