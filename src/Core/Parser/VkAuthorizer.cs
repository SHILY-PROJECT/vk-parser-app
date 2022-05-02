namespace VkParser.Core.Parser;

public class VkAuthorizer
{
    public static async Task<bool> SignIn(HttpClient client, UserDataModel userData, VkParams vkParams)
    {
        if (await VerifyCurrentStateOfAuthenticationAndCollectCookies(client))
        {
            Console.WriteLine("Account is authorized.");
            return true;
        }
        if (!await CollectTokensForSignIn(client, vkParams)) return false;
        if (!await RegisterEventSignIn(client, vkParams)) return false;

        return true;
    }

    private static async Task<bool> VerifyCurrentStateOfAuthenticationAndCollectCookies(HttpClient client)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://vk.com/")
        };
        return Regex.IsMatch(await client.HttpRequest(request), "(?<=\"user_id\":).*?(?=,)");
    }

    private static async Task<bool> CollectTokensForSignIn(HttpClient client, VkParams vkParams)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://id.vk.com/auth?" + new Dictionary<string, string>
            {
                { "app_id",         "7913379" },
                { " response_type", "silent_token" },
                { "v",              "1.46.0" },
                { "redirect_uri",   HttpUtility.UrlEncode("https://vk.com/feed") },
                {" uuid",           vkParams.Uuid }
            }
            .CreateParamsLineForUrlOrBody())
        };
        request.Headers.AddOrReplace(new() { { "Referer", "https://vk.com/" }, { "Connection", "keep-alive" } });

        return VkParamsHandler.TryExtractTokensFromHtml(vkParams, await client.HttpRequest(request));
    }

    private static async Task<bool> RegisterEventSignIn(HttpClient client, VkParams vkParams)
    {
        var eventId = new Random().Next(500000, 7000000);

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"https://api.vk.com/method/statEvents.addAnonymously?v=5.123"),
            Content = new StringContent(new Dictionary<string, string>
            {
                { "auth_token", vkParams.AuthToken },
                { "anonymous_token", vkParams.AnonymousToken },
                { "device_id", vkParams.DeviceId },
                { "service_group", "" },
                { "external_device_id", "" },
                { "source_app_id", "" },
                { "flow_type", "auth_without_password" },
                { "events", $"[{{\"id\":{eventId},\"prev_event_id\":0,\"prev_nav_id\":0,\"screen\":\"registration_phone\",\"timestamp\":\"{vkParams.Timestamp}\",\"type\":\"type_action\",\"type_action\":{{\"type\":\"type_registration_item\",\"type_registration_item\":{{\"event_type\":\"auth_start\"}}}}}}]" },
                { "access_token", "" }
            }
            .CreateParamsLineForUrlOrBody())
        };
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        request.Headers.AddOrReplace(new()
        {
            { "Referer", "https://id.vk.com/" },
            { "Origin", "https://id.vk.com" },
            { "Connection", "keep-alive" }
        });

        try
        {
            return Regex.IsMatch(await client.HttpRequest(request), "(?<=\"response\":)1.*?");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return false;
    }

    private static async Task<bool> ValidateLogin(HttpClient client, UserDataModel userData, VkParams vkParams)
    {

    }
}