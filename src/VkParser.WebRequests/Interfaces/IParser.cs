namespace VkParser.WebRequests.Interfaces;

public interface IParser
{
    Task<IEnumerable<IUser>> ParseUsersAsync(IFilterOptions filter);
}