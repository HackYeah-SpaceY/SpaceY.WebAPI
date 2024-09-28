namespace SpaceY.RestApi.Entities;

public class Chat
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string Url { get; set; } = default!;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    public string UserId { get; set; } = default!;
    public User? User { get; set; }

    public ICollection<Message>? Messages { get; set; }
    public ICollection<Screenshot>? Screenshots { get; set; }
}