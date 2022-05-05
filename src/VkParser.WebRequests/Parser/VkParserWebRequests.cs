namespace VkParser.WebRequests.Parser;

public class VkParserWebRequests : IParser
{
    private readonly IHttpClientConfiguration _httpClientConfiguration;

    public VkParserWebRequests(IHttpClientConfiguration httpClientConfiguration)
    {
        _httpClientConfiguration = httpClientConfiguration;
    }

    public async Task<IEnumerable<IUser>> ParseUsersAsync(IFilterOptions filter)
    {
        using var client = HttpHelper.CreateHttpClient(_httpClientConfiguration);
        var request = new HttpRequestMessage(HttpMethod.Get, filter.Url);
        var response = await client.SendAsync(request);

        throw new NotImplementedException();
    }
}