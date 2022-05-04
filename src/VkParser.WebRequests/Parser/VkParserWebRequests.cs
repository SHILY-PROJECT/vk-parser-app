namespace VkParser.WebRequests.Parser;

public class VkParserWebRequests : IParser
{
    private HttpClient _client;

    public VkParserWebRequests(HttpClient client)
    {
        _client = client;
    }

    public async Task<IEnumerable<IUser>> ParseUsersAsync(IFilterOptions filter)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, filter.Url);
        var response = await _client.SendAsync(request);

        throw new NotImplementedException();
    }
}