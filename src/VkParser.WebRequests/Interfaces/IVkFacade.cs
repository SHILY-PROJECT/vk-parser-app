namespace VkParser.WebRequests.Interfaces;

public interface IVkFacade
{
    Task<bool> SignInAsync();
    Task<IEnumerable<IUser>> ParseUsersAsync(IFilterOptions filter);
}