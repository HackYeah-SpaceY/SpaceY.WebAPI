using SpaceY.RestApi.Shared;
using System.Security.Claims;

namespace SpaceY.RestApi.Services;

public interface IUserContext
{
    CurrentUser? GetCurrentUser();
    string? GetCurrentUserId();

}


public class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContext;

    public UserContext(IHttpContextAccessor httpContext)
    {
        _httpContext = httpContext;

    }

    public CurrentUser? GetCurrentUser()
    {
        var user = _httpContext?.HttpContext?.User;

        if (user is null)
        {
            return null;
        }

        if (user.Identity is null || !user.Identity.IsAuthenticated)
        {
            return null;
        }


        var id = user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)!.Value;
        var email = user.FindFirst(c => c.Type == ClaimTypes.Email)!.Value;
        var userName = user.FindFirst(c => c.Type == ClaimTypes.Name)!.Value;


        return new CurrentUser(id, userName, email);
    }

    public string? GetCurrentUserId()
    {
        var user = _httpContext?.HttpContext?.User;

        if (user is null)
        {
            return null!;
        }

        if (user.Identity is null || !user.Identity.IsAuthenticated)
        {
            return null!;
        }



        return user.FindFirst(c => c.Type == ClaimTypes.NameIdentifier!)!.Value;
    }
}
