namespace VkParser.WebRequests.Models;

public class UserData : IUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}