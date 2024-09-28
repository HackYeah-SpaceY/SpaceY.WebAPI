namespace SpaceY.RestApi.Contracts.Requests;

public class AddMessageRequest
{
    public Guid ChatId { get; set; }
    public string Content { get; set; } = default!;
}
