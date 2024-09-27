namespace VkParser.WebRequests.Configuration;

public class HttpClientConfiguration : IHttpClientConfiguration
{
    private HttpClientConfiguration() { }

    public CookieContainer CookieContainer { get; private set; }
    public HttpClientHandler HttpClientHandler { get; private set; }
    public Dictionary<string, string> DefaultHeaders { get; private set; }

    public static HttpClientConfiguration CreateConfiguration()
    {
        var cfg = new HttpClientConfiguration();

        cfg.HttpClientHandler = new HttpClientHandler()
        {
            AllowAutoRedirect = true,
            MaxAutomaticRedirections = 10,
            AutomaticDecompression = DecompressionMethods.All,
            CookieContainer = cfg.CookieContainer = new()
        };
        cfg.DefaultHeaders = new()
        {
            { "Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8" },
            { "Accept-Encoding", "gzip, deflate" },
            { "Accept-Language", "en-US,en;q=0.5" },
            { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/100.0.4896.75 Safari/537.36" }
        };

        return cfg;
    }
}