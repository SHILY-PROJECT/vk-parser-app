namespace VkParser.WebRequests.Configuration;

public static class VkParserWebRequestsRegistrar
{
    public static IServiceCollection AddVkParserWebRequestsComponents(this IServiceCollection services)
    {
        var cfg = HttpClientConfiguration.CreateConfiguration();

        services
            .AddScoped<VkParams>(opt => VkParams.CreateAnObjectWithDefaultParams())
            .AddScoped<IHttpClientConfiguration>(opt => cfg)
            .AddTransient<HttpClient>(opt => HttpHelper.CreateHttpClient(cfg))
            .AddTransient<IVkFacade, WebRequestsFacade>()
            .AddTransient<IAuthorizer, VkAuthorizer>()
            .AddTransient<IParser, VkParserWebRequests>();

        return services;
    }
}