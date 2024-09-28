using Microsoft.AspNetCore.Identity;

namespace SpaceY.RestApi.Entities;

public class User : IdentityUser
{
    public IEnumerable<Chat>? Chats { get; set; }
}
