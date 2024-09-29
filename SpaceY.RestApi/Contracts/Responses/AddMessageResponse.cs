using SpaceY.RestApi.Contracts.Dtos;

namespace SpaceY.RestApi.Contracts.Responses;

public class AddMessageResponse
{
    public List<MessageDto> Messages { get; set; } = [];
    public List<ScreenshotDto> Screenshots { get; set; } = [];
}
