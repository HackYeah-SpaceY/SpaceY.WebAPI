using SpaceY.RestApi.Contracts.Dtos;

namespace SpaceY.RestApi.Contracts.Requests;

public class CreateChatRequest
{
    public string Url { get; set; } = default!;
    public string Title { get; set; } = default!;
    public MessageDto Message { get; set; } = default!;
}
