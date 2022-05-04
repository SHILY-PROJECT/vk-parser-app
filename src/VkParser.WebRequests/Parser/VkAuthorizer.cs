namespace VkParser.WebRequests.Parser;

public class VkAuthorizer : IAuthorizer
{
    private readonly HttpClient _client;
    private readonly IAccount _account;
    private readonly VkParams _vkParams;

    private readonly Dictionary<string, string> _headersForApiVk = new()
    {
        { "Referer", "https://id.vk.com/" },
        { "Origin", "https://id.vk.com" },
        { "Connection", "keep-alive" }
    };

    private int _counter = 0;

    public VkAuthorizer(HttpClient client, IAccount account, VkParams vkParams)
    {
        _client = client;
        _account = account;
        _vkParams = vkParams;
    }

    public async Task<bool> SignInAsync()
    {
        if (++_counter > 2) return false;

        if (await VerifyCurrentStateOfAuthenticationAndCollectCookiesAsync()) return true;
        if (!await CollectTokensForSignInAsync()) return false;
        if (!await RegisterEventSignInAsync()) return false;
        if (!await ValidateLoginAsync()) return false;
        if (!await RequestAuthorizationAsync()) return false;

        return await SignInAsync();
    }

    private async Task<bool> VerifyCurrentStateOfAuthenticationAndCollectCookiesAsync()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://vk.com/")
        };
        return Regex.IsMatch(await _client.HttpRequestAsync(request), "(?<=\"user_id\":).*?(?=,)");
    }

    private async Task<bool> CollectTokensForSignInAsync()
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
        request.Headers.AddRangeOrReplace(new() { { "Referer", "https://vk.com/" }, { "Connection", "keep-alive" } });

        return _vkParams.TryExtractTokensFromHtml(await _client.HttpRequestAsync(request));
    }

    private async Task<bool> RegisterEventSignInAsync()
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
        request.Headers.AddRangeOrReplace(_headersForApiVk);
        request.Content.Headers.ContentType = new("application/x-www-form-urlencoded");

        try
        {
            return Regex.IsMatch(await _client.HttpRequestAsync(request), "(?<=\"response\":)1.*?");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return false;
    }

    private async Task<bool> ValidateLoginAsync()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"https://api.vk.com/method/auth.validateAccount?v=5.174&client_id=7913379"),
            Content = new StringContent(new Dictionary<string, string>
            {
                { "login", _account.Login },
                { "sid", "" },
                { "client_id", "7913379" },
                { "auth_token", _vkParams.AuthToken },
                { "access_token", "" }
            }
            .ToParamsHttp())
        };
        request.Content.Headers.ContentType = new("application/x-www-form-urlencoded");
        request.Headers.AddRangeOrReplace(_headersForApiVk);

        return Regex.IsMatch(await _client.HttpRequestAsync(request), "(?<=\"sid\":\").*?(?=\")");
    }

    private async Task<bool> RequestAuthorizationAsync()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri($"https://login.vk.com/?act=connect_authorize"),
            Content = new StringContent(new Dictionary<string, string>
            {
                { "username", _account.Login },
                { "password", HttpUtility.UrlEncode(_account.Password) },
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
        request.Content.Headers.ContentType = new("application/x-www-form-urlencoded");
        request.Headers.AddRangeOrReplace(_headersForApiVk);

        /*
         *  TODO: Add token extraction via 'VkParamsHandler.TryExtractTokensFromHtml'
         */

        if (Regex.Match(await _client.HttpRequestAsync(request), "(?<=\"access_token\":\").*?(?=\")").Value is string accessToken && !string.IsNullOrWhiteSpace(accessToken))
        {
            _vkParams.AccessToken = accessToken;
            return true;
        }
        else return false;
    }
}