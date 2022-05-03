namespace VkParser.WebRequests.Models;

public class UserDataModel : IUser
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}