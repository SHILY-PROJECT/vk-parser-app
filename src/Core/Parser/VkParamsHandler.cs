namespace VkParser.Core.Parser;

public static class VkParamsHandler
{
    public static string CreateParamsLineForUrlOrBody(this Dictionary<string, string> dicParams) =>
        dicParams.Select((kv, index) => $"{(index != 0 ? "&" : "")}{kv.Key}={kv.Value}").Aggregate((a, b) => a + b).ToString();

    public static bool TryExtractTokensFromHtml(this VkParams vkParams, string html)
    {
        try
        {
            var rxJsonParams = new Regex(@"(?<=window\.init\ =\ )\{.*?}(?=;)");
            Map(vkParams, JsonSerializer.Deserialize<VkParamsDeserializationModel>(rxJsonParams.Match(html).Value));
            return vkParams.IsTokensFound();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return false;
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

    private static bool IsTokensFound(this VkParams vkParams) =>
        !(vkParams is null || string.IsNullOrWhiteSpace(vkParams.AuthToken) || string.IsNullOrWhiteSpace(vkParams.AnonymousToken));
}