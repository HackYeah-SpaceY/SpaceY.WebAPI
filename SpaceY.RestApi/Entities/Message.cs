namespace SpaceY.RestApi.Entities;

public class Message
{
    public Guid Id { get; set; }
    public string Content { get; set; } = default!;
    public DateTime SentAt { get; set; }
    public bool IsFromUser { get; set; }
    public Guid ChatId { get; set; }
    public Chat? Chat { get; set; }
}