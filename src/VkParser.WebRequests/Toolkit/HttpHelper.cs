namespace VkParser.WebRequests.Toolkit;

public static class HttpHelper
{
    public static void AddRange(this HttpRequestHeaders requestHeaders, Dictionary<string, string> header)
    {
        foreach (var kv in header)
        {
            requestHeaders.Add(kv.Key, kv.Value);
        }
    }

    public static void AddRangeOrReplace(this HttpRequestHeaders requestHeaders, Dictionary<string, string> headers)
    {
        foreach (var kv in headers)
        {
            if (requestHeaders.Contains(kv.Key)) requestHeaders.Remove(kv.Key);
            requestHeaders.Add(kv.Key, kv.Value);
        }
    }

    public static void AddOrReplace(this HttpRequestHeaders requestHeaders, string name, string value)
    {
        if (requestHeaders.Contains(name)) requestHeaders.Remove(name);
        requestHeaders.Add(name, value);
    }

    public static HttpClient CreateHttpClient(IHttpClientConfiguration httpClientConfiguration)
    {
        var client = new HttpClient(httpClientConfiguration.HttpClientHandler, false);
        client.DefaultRequestHeaders.AddRange(httpClientConfiguration.DefaultHeaders);
        return client;
    }

    public static async Task<string> HttpRequestAsync(this HttpClient client, HttpRequestMessage httpRequest)
    {
        var resp = await client.SendAsync(httpRequest);
        using var streamReader = new StreamReader(await resp.Content.ReadAsStreamAsync(), Encoding.GetEncoding("iso-8859-1"));
        return await streamReader.ReadToEndAsync();
    }
}