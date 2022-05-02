namespace VkParser.Core.Parser;

public class VkAuthorizer : IAuthorizer
{
    private int _counter = 0;

    private readonly HttpClient _client;
    private readonly UserDataModel _userData;
    private readonly VkParams _vkParams;

    public VkAuthorizer(HttpClient client, UserDataModel userData, VkParams vkParams)
    {
        _client = client;
        _userData = userData;
        _vkParams = vkParams;
    }

    public async Task<bool> SignIn()
    {
        if (++_counter > 2) return false;

        if (await VerifyCurrentStateOfAuthenticationAndCollectCookies()) return true;
        if (!await CollectTokensForSignIn()) return false;
        if (!await RegisterEventSignIn()) return false;
        if (!await ValidateLogin()) return false;
        if (!await RequestAuthorization()) return false;

        return await SignIn();
    }

    private async Task<bool> VerifyCurrentStateOfAuthenticationAndCollectCookies()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://vk.com/")
        };
        return Regex.IsMatch(await _client.HttpRequest(request), "(?<=\"user_id\":).*?(?=,)");
    }

    private async Task<bool> CollectTokensForSignIn()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://id.vk.com/auth?" + new Dictionary<string, string>
            {
                { "app_id", "7913379" },
                { " response_type", "silent_token" },
                { "v", "1.46.0" },
                { "redirect_uri", HttpUtility.UrlEncode("https://vk.com/feed") },
                {" uuid", _vkParams.Uuid }
            }
            .ToParamsHttp())
        };
        request.Headers.AddOrReplace(new() { { "Referer", "https://vk.com/" }, { "Connection", "keep-alive" } });

        return VkParamsHandler.TryExtractTokensFromHtml(_vkParams, await _client.HttpRequest(request));
    }

    private async Task<bool> RegisterEventSignIn()
    {
        var eventId = new Random().Next(500000, 7000000);

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"https://api.vk.com/method/statEvents.addAnonymously?v=5.123"),
            Content = new StringContent(new Dictionary<string, string>
            {
                { "auth_token", _vkParams.AuthToken },
                { "anonymous_token", _vkParams.AnonymousToken },
                { "device_id", _vkParams.DeviceId },
                { "service_group", "" },
                { "external_device_id", "" },
                { "source_app_id", "" },
                { "flow_type", "auth_without_password" },
                { "events", $"[{{\"id\":{eventId},\"prev_event_id\":0,\"prev_nav_id\":0,\"screen\":\"registration_phone\",\"timestamp\":\"{_vkParams.Timestamp}\",\"type\":\"type_action\",\"type_action\":{{\"type\":\"type_registration_item\",\"type_registration_item\":{{\"event_type\":\"auth_start\"}}}}}}]" },
                { "access_token", "" }
            }
            .ToParamsHttp())
        };
        request.Headers.AddOrReplace(new()
        {
            { "Referer", "https://id.vk.com/" },
            { "Origin", "https://id.vk.com" },
            { "Connection", "keep-alive" }
        });
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");


        try
        {
            return Regex.IsMatch(await _client.HttpRequest(request), "(?<=\"response\":)1.*?");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return false;
    }

    private async Task<bool> ValidateLogin()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"https://api.vk.com/method/auth.validateAccount?v=5.174&client_id=7913379"),
            Content = new StringContent(new Dictionary<string, string>
            {
                { "login", _userData.Login },
                { "sid", "" },
                { "client_id", "7913379" },
                { "auth_token", _vkParams.AuthToken },
                { "access_token", "" }
            }
            .ToParamsHttp())
        };
        request.Headers.AddOrReplace(new()
        {
            { "Referer", "https://id.vk.com/" },
            { "Origin", "https://id.vk.com" },
            { "Connection", "keep-alive" }
        });
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        return Regex.IsMatch(await _client.HttpRequest(request), "(?<=\"sid\":\").*?(?=\")");
    }

    private async Task<bool> RequestAuthorization()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"https://login.vk.com/?act=connect_authorize"),
            Content = new StringContent(new Dictionary<string, string>
            {
                { "username", _userData.Login },
                { "password", HttpUtility.UrlEncode(_userData.Password) },
                { "auth_token", _vkParams.AuthToken },
                { "sid", "" },
                { "uuid", _vkParams.Uuid },
                { "v", "5.174" },
                { "device_id", _vkParams.DeviceId },
                { "service_group", "" },
                { "version", "1" },
                { "app_id", "7913379" },
                { "access_token", "" }
            }
            .ToParamsHttp())
        };
        request.Headers.AddOrReplace(new()
        {
            { "Referer", "https://id.vk.com/" },
            { "Origin", "https://id.vk.com" },
            { "Connection", "keep-alive" }
        });
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        /*
         *  TODO: Add token extraction via 'VkParamsHandler.TryExtractTokensFromHtml'
         */

        if (Regex.Match(await _client.HttpRequest(request), "(?<=\"access_token\":\").*?(?=\")").Value is string accessToken && !string.IsNullOrWhiteSpace(accessToken))
        {
            _vkParams.AccessToken = accessToken;
            return true;
        }
        else return false;
    }
}