namespace VkParser.Core.Models;

public class VkParamsDeserializationModel
{
    [JsonPropertyName("auth_token")]
    public string AuthToken { get; set; } = string.Empty;

    [JsonPropertyName("anonymous_token")]
    public string AnonymousToken { get; set; } = string.Empty;
}