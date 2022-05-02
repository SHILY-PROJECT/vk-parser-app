namespace VkParser.Core.Interfaces;

public interface IAuthorizer
{
    Task<bool> SignIn();
}