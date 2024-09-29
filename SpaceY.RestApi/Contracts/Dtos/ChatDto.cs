using SpaceY.RestApi.Entities;

namespace SpaceY.RestApi.Contracts.Dtos;

public class ChatDto
{
    public string? Title { get; set; }
    public string Url { get; set; } = default!;
    public bool IsArchived { get; set; }
    public DateTime ModifiedAt { get; set; }

    public ICollection<MessageDto>? Messages { get; set; }
    public ICollection<ScreenshotDto>? Screenshots { get; set; }
}
