namespace SpaceY.RestApi.Contracts.Dtos;

public class MessageDto
{
    public string Content { get; set; } = default!;
    public bool IsFromUser { get; set; }
    public DateTime SentAt { get; set; }
}
