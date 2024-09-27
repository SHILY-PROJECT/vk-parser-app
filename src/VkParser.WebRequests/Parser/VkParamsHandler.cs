namespace VkParser.WebRequests.Parser;

public static class VkParamsHandler
{
    public static string ToParamsHttp(this Dictionary<string, string> dicParams) =>
        dicParams.Select((kv, index) => $"{(index != 0 ? "&" : "")}{kv.Key}={kv.Value}").Aggregate((a, b) => a + b).ToString();

    public static bool TryExtractTokensFromHtml(this VkParams vkParams, string html)
    {
        var flag = default(bool);

        try
        {
            /*
             *  TODO: Need to fix 'AccessToken' receipt.
             */
            var rxJsonParams = new Regex("((?<=window\\.init\\ =\\ )\\{.*?}(?=;)|(?<=\"data\":)\\{.*?})");
            var deserializedParams = JsonSerializer.Deserialize<VkParamsDeserializationModel>(rxJsonParams.Match(html).Value);

            flag = deserializedParams switch
            {
                _ when deserializedParams != null && !string.IsNullOrWhiteSpace(deserializedParams.AccessToken) => true,
                _ when deserializedParams != null && !string.IsNullOrWhiteSpace(deserializedParams.AuthToken) && !string.IsNullOrWhiteSpace(deserializedParams.AnonymousToken) => true,
                _ => false
            };

            if (flag) Map(vkParams, deserializedParams);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return flag;
    }

    private static void Map(VkParams? vkParams, VkParamsDeserializationModel? vkParamsDeserialization)
    {
        if (vkParams is null) throw new ArgumentNullException(nameof(vkParams));
        if (vkParamsDeserialization is null) throw new ArgumentNullException(nameof(vkParamsDeserialization));

        foreach (var prop in typeof(VkParamsDeserializationModel).GetProperties())
        {
            if (prop.GetValue(vkParamsDeserialization) is object value && (value.GetType() != typeof(string) || !string.IsNullOrWhiteSpace(value as string)))
            {
                typeof(VkParams)?.GetProperty(prop.Name)?.SetValue(vkParams, value);
            }
        }
    }
}