namespace VkParser.WebRequests.Interfaces;

public interface IAuthorizer
{
    Task<bool> SignInAsync();
}