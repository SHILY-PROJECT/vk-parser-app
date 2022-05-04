namespace VkParser.WebRequests.Parser;

public class VkParserWebRequests : IParser
{
    private HttpClient _client;

    public VkParserWebRequests(HttpClient client)
    {
        _client = client;
    }

    public async Task<IEnumerable<IUser>> ParseUsersAsync(IParsingOptions options)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, options.Url);
        var response = await _client.SendAsync(request);

        throw new NotImplementedException();
    }
}