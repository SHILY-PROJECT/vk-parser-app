namespace VkParser.Core.Models;

public class VkParams
{
    private VkParams() { }

    public string Uuid { get; set; } = string.Empty;
    public string AuthToken { get; set; } = string.Empty;
    public string AnonymousToken { get; set; } = string.Empty;
    public string DeviceId { get; set; } = string.Empty;
    public string Timestamp { get => (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds.ToString().Replace(".", "")[..13]; }

    public static VkParams CreateEmptyObject() => new();

    public static VkParams CreateAnObjectWithDefaultParameters() => new()
    {
        DeviceId = TextMacros.RandomString(21, "abc"),
        Uuid = TextMacros.RandomString(21, "abc")
    };
}