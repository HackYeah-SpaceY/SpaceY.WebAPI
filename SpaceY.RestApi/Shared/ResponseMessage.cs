using System.Text.Json.Serialization;

namespace SpaceY.RestApi.Shared;

public class ResponseMessage
{
    [JsonPropertyName("response")]
    public string? Content { get; set; } = default!;
    [JsonPropertyName("screenshot")]
    public string? ScreenshotPath { get; set; } = default!;
    [JsonPropertyName("status")]
    public string? Status { get; set; } = default!;
    [JsonPropertyName("error")]
    public string? Error { get; set; } = default!;
}
