namespace SpaceY.RestApi.Entities;

public class Screenshot
{
    public Guid Id { get; set; }
    public string FilePath { get; set; } = default!;
    public DateTime CreatedAt { get; set; }

    public Guid ChatId { get; set; }
    public Chat? Chat { get; set; }
}