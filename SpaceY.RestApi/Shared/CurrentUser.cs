namespace SpaceY.RestApi.Shared;

public class CurrentUser
{
    public CurrentUser(string id, string userName, string email)
    {
        Id = id;
        Email = email;
        UserName = userName;
    }

    public string Id { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }

}