namespace VkParser.ConsoleUI.Configuration;

public static class ConsoleUIRegistrar
{
    public static IServiceCollection AddConsoleUIComponents(this IServiceCollection services)
    {
        Console.InputEncoding = Encoding.UTF8;
        Console.OutputEncoding = Encoding.UTF8;

        services
            .AddSingleton<Startup>()
            .AddSingleton<IAccount, AccountDataModel>()
            .AddVkParserWebRequestsComponents();

        return services;
    }
}