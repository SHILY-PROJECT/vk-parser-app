namespace VkParser.Core.Toolkit;

public static class HttpHelper
{
    public static void Add(this HttpRequestHeaders requestHeaders, Dictionary<string, string> addHeaders)
    {
        foreach (var kv in addHeaders)
        {
            requestHeaders.Add(kv.Key, kv.Value);
        }
    }

    public static void AddOrReplace(this HttpRequestHeaders requestHeaders, Dictionary<string, string> addHeaders)
    {
        foreach (var kv in addHeaders)
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

    public static async Task<string> HttpRequest(this HttpClient client, HttpRequestMessage httpRequest)
    {
        var resp = await client.SendAsync(httpRequest);
        using var streamReader = new StreamReader(await resp.Content.ReadAsStreamAsync(), Encoding.GetEncoding("iso-8859-1"));
        return await streamReader.ReadToEndAsync();
    }
    
}