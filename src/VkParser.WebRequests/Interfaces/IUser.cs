namespace VkParser.WebRequests.Interfaces;

public interface IUser
{
    string FirstName { get; set; }
    string LastName { get; set; }
    string Url { get; set; }
}