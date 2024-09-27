namespace VkParser.WebRequests.Models;

public class VkParamsDeserializationModel
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("auth_token")]
    public string AuthToken { get; set; } = string.Empty;

    [JsonPropertyName("anonymous_token")]
    public string AnonymousToken { get; set; } = string.Empty;
}