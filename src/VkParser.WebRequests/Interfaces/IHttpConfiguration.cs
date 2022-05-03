namespace VkParser.WebRequests.Interfaces;

public interface IHttpConfiguration
{
    HttpClientHandler? HttpClientHandler { get; }
    CookieContainer? CookieContainer { get; }
    Dictionary<string, string>? DefaultHeaders { get; }
}