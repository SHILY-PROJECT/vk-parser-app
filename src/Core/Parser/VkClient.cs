namespace VkParser.Core.Parser;

internal class VkClient
{
    private readonly Dictionary<string, string> _defaultHeaders = new()
    {
        {"Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8" },
        {"Accept-Encoding", "gzip, deflate" },
        {"Accept-Language", "en-US,en;q=0.5" },
        { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.75 Safari/537.36" }
    };

    private readonly HttpClientHandler _httpHandler;
    private readonly CookieContainer _cookieContainer;
    private readonly VkParams _params;

    public VkClient()
    {
        _params = VkParams.CreateAnObjectWithDefaultParameters();
        _cookieContainer = new();

        _httpHandler = new()
        {
            AllowAutoRedirect = true,
            MaxAutomaticRedirections = 10,
            AutomaticDecompression = DecompressionMethods.All,
            CookieContainer = _cookieContainer
        };
    }

    public async Task<bool> Auth(UserDataModel authentication)
    {
        using var client = new HttpClient(_httpHandler);
        client.DefaultRequestHeaders.Add(_defaultHeaders);

        await VkAuthorizer.SignIn(client, authentication, _params);

        return true;
    }
}