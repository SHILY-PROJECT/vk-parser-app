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
            
            /*
             *  TODO: Добавить маппинг
             */

            return vkParams.IsTokensFound();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return false;
    }

    private static bool IsTokensFound(this VkParams vkParams) =>
        !(vkParams is null || string.IsNullOrWhiteSpace(vkParams.AuthToken) || string.IsNullOrWhiteSpace(vkParams.AnonymousToken));
}