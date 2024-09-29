namespace SpaceY.RestApi.Contracts.Dtos;

public class ChatSummaryDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public bool IsArchived { get; set; }
}
