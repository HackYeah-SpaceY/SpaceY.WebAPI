namespace SpaceY.RestApi.Contracts.Dtos;

public class ScreenshotDto
{
    public Guid Id { get; set; }
    public string FilePath { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}
