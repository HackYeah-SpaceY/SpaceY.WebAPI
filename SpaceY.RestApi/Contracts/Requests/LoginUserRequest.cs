namespace SpaceY.RestApi.Contracts.Requests;

public class LoginUserRequest
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
}
