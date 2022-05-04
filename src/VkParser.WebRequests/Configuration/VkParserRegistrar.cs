namespace VkParser.WebRequests.Configuration;

public static class VkParserRegistrar
{
    public static IServiceCollection AddVkParserComponents(this IServiceCollection services)
    {
        var cfg = HttpClientConfiguration.CreateConfiguration();

        services
            .AddScoped<VkParams>(opt => VkParams.CreateAnObjectWithDefaultParams())
            .AddScoped<IHttpClientConfiguration>(opt => cfg)
            .AddTransient<HttpClient>(opt => ConfigureHttpClient(cfg))
            .AddTransient<IVkFacade, WebRequestsFacade>()
            .AddTransient<IAuthorizer, VkAuthorizer>()
            .AddTransient<IParser, VkParserWebRequests>();

        return services;
    }

    private static HttpClient ConfigureHttpClient(IHttpClientConfiguration cfg)
    {
        var client = new HttpClient(cfg.HttpClientHandler);
        client.DefaultRequestHeaders.AddRange(cfg.DefaultHeaders);
        return client;
    }
}