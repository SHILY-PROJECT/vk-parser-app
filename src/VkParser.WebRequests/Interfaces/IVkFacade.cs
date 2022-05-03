namespace VkParser.WebRequests.Interfaces;

public interface IVkFacade
{
    Task<bool> Auth();
    Task<IEnumerable<IPerson>> ParsePersonsByUrl(Uri url);
}